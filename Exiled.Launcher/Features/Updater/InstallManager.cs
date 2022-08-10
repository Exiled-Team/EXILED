// -----------------------------------------------------------------------
// <copyright file="InstallManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Net;
using System.Text;
using System.Text.Json;

using Exiled.Launcher.Features.Arguments;
using Exiled.Launcher.Features.Updater.GithubAPI;

namespace Exiled.Launcher.Features.Updater;

public class InstallManager : IDisposable
{
    private const string RepoID = "Exiled-Team/Exiled";
    private const string ExiledAssetName = "Exiled.tar.gz";

    private readonly bool preReleases;
    private readonly string desiredVersion;

    private readonly HttpClient httpClient = new ();
    private List<GithubWrapper.Release> releases = new ();

    public InstallManager(LauncherArguments arguments)
    {
        preReleases = arguments.DownloadPreReleases;
        desiredVersion = arguments.ExiledVersion;

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Exiled-Launcher");
        if (!string.IsNullOrEmpty(arguments.GithubToken))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization",  "Basic " + Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(arguments.GithubToken)));
        }
    }

    public bool TryInstall()
    {
        Console.WriteLine("Installing Exiled...");

        if (!TryFetchReleases())
            return false;

        Console.WriteLine($"Finding release with Id or Tag: {desiredVersion}");

        GithubWrapper.Release? target = TryGetTargetRelease(desiredVersion);

        if (target is null)
        {
            Console.WriteLine($"Couldn't find release with Tag or Id: {desiredVersion}.");
            Console.WriteLine("Available releases:");

            foreach (var release in releases)
            {
                Console.WriteLine($"Id: {release.Id} | Tag: {release.TagName} | Name: {release.Name} | Prerelease: {release.Prerelease} | Publish Date: {release.PublishedAt}");
            }

            return false;
        }

        Console.WriteLine($"- Found the desired release. Name: {target.Name} | Version: {target.TagName} | Prerelease: {target.Prerelease}");
        Console.WriteLine("Installing release.");

        GithubWrapper.Asset? asset =  target.Assets.FirstOrDefault(x => x.Name == ExiledAssetName);

        if (asset is null)
        {
            Console.WriteLine("Couldn't find the file Exiled.tar.gz in the release.");
            return false;
        }

        try
        {
            Console.WriteLine("Downloading the release.");
            string tmpDownload = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ExiledAssetName);
            Console.WriteLine("Download url: " + asset.BrowserDownloadUrl);
            using HttpResponseMessage res = httpClient.GetAsync(asset.BrowserDownloadUrl).GetAwaiter().GetResult();

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Console.WriteLine("You have been rate limited by github, therefore Exiled couldn't be installed.");
                Console.WriteLine("Skipping installation.");
                return false;
            }

            Console.WriteLine("Writing the file.");

            using (var fs = File.OpenWrite(tmpDownload))
            {
                res.Content.ReadAsStream().CopyTo(fs);
            }

            File.WriteAllBytes(tmpDownload, res.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult());
            Console.WriteLine("Extracting the file.");
            TarGzExtractor.ExtractTarGz(tmpDownload, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            Console.WriteLine("Removing unnecessary files");
            File.Delete(tmpDownload);
            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Assembly-CSharp.dll"));
            Console.WriteLine("Installation completed!");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while downloading the release:");
            Console.WriteLine(e);
            return false;
        }
    }

    private bool TryFetchReleases()
    {
        Console.WriteLine("Fetching Releases:");
        try
        {
            using HttpResponseMessage res
                = httpClient.GetAsync($"https://api.github.com/repos/{RepoID}/releases?per_page=100").GetAwaiter().GetResult();

            if (res.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Console.WriteLine("You have been ratelimited by github, therefore Exiled couldn't be installed.");
                Console.WriteLine("Skyping installation.");
                return false;
            }

            string response = res.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            releases = JsonSerializer.Deserialize<List<GithubWrapper.Release>>(response)!;
            Console.WriteLine($"- Found {releases.Count} releases.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred while fetching the releases:");
            Console.WriteLine(e);
            return false;
        }
    }

    private GithubWrapper.Release? TryGetTargetRelease(string tag)
    {
        if (releases.Count == 0)
            return null;

        if (tag is "latest")
        {
            if (preReleases)
                return releases[0];

            for (int i = 0; i < releases.Count; i++)
            {
                if (!releases[i].Prerelease)
                    return releases[i];
            }
        }

        foreach (var release in releases)
            if (release.TagName == tag || release.Id.ToString() == tag)
                return release;

        return null;
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}

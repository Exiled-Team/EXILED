---
uid: contributing
---
# Contributing

## Contributing code

### Forking EXILED
First, create a fork of our [GitHub repository](https://github.com/Exiled-Team/EXILED).

Then, clone it to your computer like so: `git clone https://github.com/your-username/EXILED.git`

Open a terminal in your forked EXILED folder and run ```git checkout dev```. This will switch you to the dev branch, which all pull requests should be submitted to.

### Setting `EXILED_REFERENCES`

If you haven't already, install the `SCP: Secret Laboratory Dedicated Server` through Steam or extract [this zip file](https://exiled.host/build_deps/References.zip) to an easily accessible folder.

#### Windows users
Open the Environment Variables menu by searching for `Environment Variables` in the Start Menu.

Create a new environment variable titled `EXILED_REFERENCES`.

The value should point to `your_steamapps_directory/common/SCP Secret Laboratory Dedicated Server/SCPSL_Data/Managed`, or to the folder where you extracted the zip file mentioned earlier.

#### Linux users
Add `export EXILED_REFERENCES="PATH"` to your `~/.bashrc` or similar file.

PATH should point to `your_steamapps_directory/common/SCP Secret Laboratory Dedicated Server/SCPSL_Data/Managed`, or to the folder where you extracted the zip file mentioned earlier.

---

You should now be able to open the EXILED directory in your favorite IDE.


Once you are done, test your changes thoroughly, and then submit a pull request to the main EXILED repository. Make sure you are targeting the `dev` branch, not `master`!

Happy coding!

## Contributing docs
Documentation is built using [DocFX.](https://dotnet.github.io/docfx/index.html)

### Forking EXILED
First, create a fork of our [GitHub repository](https://github.com/Exiled-Team/EXILED).

Then, clone it to your computer like so: ```git clone https://github.com/your-username/EXILED.git```

Open a terminal in your forked EXILED folder and run ```git checkout dev```. This will switch you to the dev branch, which all pull requests should be submitted to.

### DocFX installation
If you have Chocolatey installed, installation is simple as ```choco install docfx```

Homebrew installation is just as simple: ```brew install docfx```

You can also get it via NuGet: ```nuget install docfx.console```

### Writing and building docs

All of our articles live in the docs/ directory in the root of the repository. To make a new article, simply create a new Markdown (.md) file in the docs/articles folder with a descriptive name. Then, add it to the toc.yml located in that same folder. For more information on toc.yml formatting, check the [DocFX documentation.](https://dotnet.github.io/docfx/tutorial/intro_toc.html#yaml-format-toc-tocyml)

You can then write your article using Markdown. For more information on Markdown syntax, check out [Markdown Guide.](https://www.markdownguide.org/)

To build documentation, run ```docfx docfx.json``` and the completed documentation will be generated in `_site/`. Do not push `_site` to GitHub, as GitHub Actions will generate updated documentation when it is pushed to master.

You can then open `_site/index.html` in your favorite web browser to preview the results.

Once you are done, submit a pull request to the main EXILED repository. Make sure you are targeting the dev branch, not master!


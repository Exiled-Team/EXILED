// -----------------------------------------------------------------------
// <copyright file="ThreadSafeRequest.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Features
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using MEC;

    using UnityEngine;

    internal sealed class ThreadSafeRequest
    {
        /// <summary>
        /// Handles the Safe Thread Request.
        /// </summary>
        private volatile bool done;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// Gets or sets if it was successful.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets or sets the HTTP Status Code.
        /// </summary>
        public HttpStatusCode Code { get; private set; }

        /// <summary>
        /// True/False if Done was successful.
        /// </summary>
        public bool Done => done;
        /// <summary>
        /// Gets the call to the website to obtain users to their roles.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="errorHandler"></param>
        /// <param name="resultHandler"></param>
        /// <param name="issuer"></param>
        public static void Go(string url, Action<ThreadSafeRequest> errorHandler, Action<string> resultHandler, GameObject issuer)
        {
            Timing.RunCoroutine(MakeRequest(url, errorHandler, resultHandler).CancelWith(issuer), Segment.LateUpdate);
        }

        private static IEnumerator<float> MakeRequest(string url, Action<ThreadSafeRequest> errorHandler, Action<string> resultHandler)
        {
            ThreadSafeRequest request = new();

            Task.Run(
                () =>
                {
                    request.Result = HttpQuery.Get(url, out bool success, out HttpStatusCode code);
                    request.Success = success;
                    request.Code = code;

                    request.done = true;
                });

            yield return Timing.WaitUntilTrue(() => request.done);

            if (request.Success)
                resultHandler(request.Result);
            else
                errorHandler(request);
        }
    }
}
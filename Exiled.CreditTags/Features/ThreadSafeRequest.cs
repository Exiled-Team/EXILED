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
        /// Gets the result.
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not it was successful.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the HTTP Status Code.
        /// </summary>
        public HttpStatusCode Code { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the request was successful.
        /// </summary>
        public bool Done => done;

        /// <summary>
        /// Gets the call to the website to obtain users to their roles.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="errorHandler">The error handling <see cref="Action{T1}"/>.</param>
        /// <param name="resultHandler">The result handling <see cref="Action{T1}"/>.</param>
        /// <param name="issuer">The <see cref="GameObject"/> issuing the request.</param>
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
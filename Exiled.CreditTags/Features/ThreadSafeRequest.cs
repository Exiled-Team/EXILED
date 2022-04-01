// -----------------------------------------------------------------------
// <copyright file="ThreadSafeRequest.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Features {
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using MEC;

    using UnityEngine;

    internal sealed class ThreadSafeRequest {
        private volatile bool done;

        public string Result { get; private set; }

        public bool Success { get; private set; }

        public HttpStatusCode Code { get; private set; }

        public bool Done => done;

        public static void Go(string url, Action<ThreadSafeRequest> errorHandler, Action<string> resultHandler, GameObject issuer) {
            Timing.RunCoroutine(MakeRequest(url, errorHandler, resultHandler).CancelWith(issuer), Segment.LateUpdate);
        }

        private static IEnumerator<float> MakeRequest(string url, Action<ThreadSafeRequest> errorHandler, Action<string> resultHandler) {
            ThreadSafeRequest request = new ThreadSafeRequest();

            Task.Run(() => {
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

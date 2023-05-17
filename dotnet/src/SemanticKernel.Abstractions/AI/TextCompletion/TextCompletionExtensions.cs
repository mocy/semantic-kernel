﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.SemanticKernel.AI.TextCompletion;

/// <summary>
/// Class sponsor that holds extension methods for ITextCompletion interface.
/// </summary>
public static class TextCompletionExtensions
{
    /// <summary>
    /// Creates a completion for the prompt and settings.
    /// </summary>
    /// <param name="textCompletion">Target interface to extend</param>
    /// <param name="text">The prompt to complete.</param>
    /// <param name="requestSettings">Request settings for the completion API</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Text generated by the remote model</returns>
    public static async Task<string> CompleteAsync(this ITextCompletion textCompletion,
        string text,
        CompleteRequestSettings requestSettings,
        CancellationToken cancellationToken = default)
    {
        var completions = await textCompletion.GetCompletionsAsync(text, requestSettings, cancellationToken).ConfigureAwait(false);

        StringBuilder completionResult = new();

        foreach (ITextCompletionResult result in completions)
        {
            completionResult.Append(await result.GetCompletionAsync(cancellationToken).ConfigureAwait(false));
        }

        return completionResult.ToString();
    }

    /// <summary>
    /// Creates a completion for the prompt and settings.
    /// </summary>
    /// <param name="textCompletion">Target interface to extend</param>
    /// <param name="text">The prompt to complete.</param>
    /// <param name="requestSettings">Request settings for the completion API</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>Streaming content of the text generated by the remote model</returns>
    public static async IAsyncEnumerable<string> CompleteStreamAsync(this ITextCompletion textCompletion,
        string text,
        CompleteRequestSettings requestSettings,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var completionResults = textCompletion.GetStreamingCompletionsAsync(text, requestSettings, cancellationToken);

        await foreach (var completionResult in completionResults.ConfigureAwait(false))
        {
            await foreach (var word in completionResult.GetCompletionStreamingAsync(cancellationToken).ConfigureAwait(false))
            {
                yield return word;
            }
        }
    }
}

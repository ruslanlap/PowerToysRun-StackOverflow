using System;
using System.Linq;
using Community.PowerToys.Run.Plugin.StackOverflow.Models;

namespace Community.PowerToys.Run.Plugin.StackOverflow.Formatters
{
    /// <summary>
    /// Formats StackOverflow questions for display in PowerToys Run.
    /// </summary>
    public class ResultFormatter
    {
        private const int MaxTagsToDisplay = 5;

        /// <summary>
        /// Formats the question title for display.
        /// </summary>
        public string FormatTitle(StackOverflowQuestion question)
        {
            return question.Title;
        }

        /// <summary>
        /// Formats the subtitle with score, answer count, accepted answer indicator, and tags.
        /// </summary>
        public string FormatSubtitle(StackOverflowQuestion question)
        {
            var parts = new System.Collections.Generic.List<string>();

            // Add score
            var scoreIcon = question.Score >= 0 ? "↑" : "↓";
            parts.Add($"{scoreIcon}{Math.Abs(question.Score)}");

            // Add answer count
            var answerText = question.AnswerCount == 1 ? "answer" : "answers";
            var answerPart = question.AnswerCount == 0 
                ? "no answers" 
                : $"{question.AnswerCount} {answerText}";
            
            if (question.HasAcceptedAnswer)
            {
                answerPart += " ✓";
            }
            
            parts.Add(answerPart);

            // Add tags (limit to first N tags)
            if (question.Tags != null && question.Tags.Length > 0)
            {
                var tagsToShow = question.Tags.Take(MaxTagsToDisplay);
                parts.Add(string.Join(" ", tagsToShow));
            }

            return string.Join(" • ", parts);
        }

        /// <summary>
        /// Formats tooltip text with additional details.
        /// </summary>
        public string FormatTooltip(StackOverflowQuestion question)
        {
            return $"{question.Title}\n\nScore: {question.Score}\nAnswers: {question.AnswerCount}\nTags: {string.Join(", ", question.Tags)}";
        }
    }
}

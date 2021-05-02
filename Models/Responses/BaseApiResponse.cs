using System.Collections.Generic;

namespace UserManagement_Backend.Models.Responses
{
    public class BaseApiResponse
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public IEnumerable<dynamic> Data { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }

    public static class BaseApiResponseHelper
    {
        #region Public Methods
        public static BaseApiResponse GenerateApiResponse(bool succeeded, string textToDisplay, IEnumerable<dynamic> data, IEnumerable<string> errors)
        {
            var message = MessageFormatter(succeeded, textToDisplay);

            return new BaseApiResponse
            {
                Succeeded = succeeded,
                Message = message,
                Data = data,
                Errors = errors
            };
        }
        #endregion

        #region Private Methods
        private static string MessageFormatter(bool succeeded, string textToDisplay)
        {
            if (succeeded)
            {
                return $"{textToDisplay} successfully.";
            }
            else
            {
                return $"An error occurred when attempting to {textToDisplay}";
            }
        }
        #endregion
    }
}

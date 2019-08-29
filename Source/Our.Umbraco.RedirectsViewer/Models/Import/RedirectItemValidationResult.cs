namespace Our.Umbraco.RedirectsViewer.Models.Import
{
    public class RedirectItemValidationResult
    {
        public string ErrorMessage { get; set; }

        public ImportErrorLevel Status { get; set; }
    }
}
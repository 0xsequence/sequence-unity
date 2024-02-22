namespace Sequence.WaaS
{
    public class IntentResponseValidationFinished
    {
        public bool isValid { get; private set; }
        
        public IntentResponseValidationFinished(bool isValid)
        {
            this.isValid = isValid;
        }
    }
}
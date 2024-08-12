namespace Domain.Entities.Response;

public class TransactionResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string? ErrorMessage => !IsSuccess ? Message : null;

    public TransactionResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static TransactionResult Success(string message = "Transaction completed successfully.")
    {
        return new TransactionResult(true, message);
    }

    public static TransactionResult Failure(string errorMessage)
    {
        return new TransactionResult(false, errorMessage);
    }
}

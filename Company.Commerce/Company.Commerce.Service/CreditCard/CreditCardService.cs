using AuthorizeNet;
using Framework.Repository;
using Project.MVC.Entity;
using Project.MVC.Entity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class CreditCardAuthorizationResult : OperationResult
    {
        public CreditCardAuthorizationResult() : base() { }

        public CreditCardTransaction Transaction { get; set; }

        //public String authorizationCode { get; set; }

        //public String transactionID { get; set; }

        //public TransactionProcessor transactionProcessor { get; set; }
    }

    public class CreditCardCaptureResult : OperationResult
    {
        public CreditCardCaptureResult() : base() { }
    }

    public interface ICreditCardService
    {
        CreditCardAuthorizationResult Authorize(CreditCardPayment payment, String description, Project.MVC.Entity.Models.Address billingAddress);

        CreditCardCaptureResult Capture(CreditCardTransaction transaction);
    }

    public enum AuthorizeNetResponseCode
    {
        Default,
        Approved,
        Declined,
        Error,
        UnderReview
    }

    public class AuthorizeNetAuthorizationResult : OperationResult
    {
        public AuthorizeNetAuthorizationResult() : base() { }

        public Decimal Amount { get; set; }

        public String AuthorizationCode { get; set; }

        public String AuthorizeNetResponseMessage { get; set; }

        public String MerchantTransactionID { get; set; }

        public AuthorizeNetResponseCode ResponseCode { get; set; }

        public Int32 ReasonCode { get; set; }
    }

    public class AuthorizeNetCaptureResult : OperationResult
    {
        public AuthorizeNetCaptureResult() : base() { }
    }

    public class AuthorizeNetService
    {
        private List<Int32> _userFriendlyAuthorizationCodes;

        private readonly String _apiLoginID;
        private readonly String _transactionKey;

        public AuthorizeNetService()
        {
            _apiLoginID = "5hVHQz84B";
            _transactionKey = "653Dh9rH3W88juYU";

            popualteUserFriendlyAuthorizationCodes();
        }

        public AuthorizeNetAuthorizationResult Authorize(String creditCardNumber, String expiration, String cvv, Decimal amount, String description, Project.MVC.Entity.Models.Address billingAddress)
        {
            if (billingAddress == null)
                throw new ArgumentNullException("billingAddress");

            if (String.IsNullOrEmpty(creditCardNumber))
                throw new ArgumentNullException("creditCardNumber");

            if (String.IsNullOrEmpty(cvv))
                throw new ArgumentNullException("cvv");

            if (String.IsNullOrEmpty(expiration))
                throw new ArgumentNullException("expiration");

            if (amount <= 0)
                throw new InvalidOperationException("Attempt to authorize by an amount less than or equal to 0.");

            var result = new AuthorizeNetAuthorizationResult();

            try
            {
                var request = new AuthorizationRequest(creditCardNumber, expiration, amount, description);

                request.Type = "AUTH_ONLY";

                //Cvv number
                request.AddCardCode(cvv);
                //No Partial Auth
                request.AllowPartialAuth = "false";
                //20 minute duplicate window
                request.DuplicateWindow = "1200";

                //Billing Address
                request.Address = billingAddress.AddressLine1;
                request.City = billingAddress.City;
                request.State = billingAddress.State;
                request.Zip = billingAddress.PostalCode;
                request.Country = "US";

                var gate = new Gateway(_apiLoginID, _transactionKey);

#if DEBUG
                request.DuplicateWindow = "1";
#endif

                IGatewayResponse response = null;

                response = gate.Send(request);

                result.ResponseCode = (AuthorizeNetResponseCode)Enum.Parse(typeof(AuthorizeNetResponseCode), response.ResponseCode);

                result.AuthorizeNetResponseMessage = response.Message;

                //If the transaction was approved
                if (result.ResponseCode == AuthorizeNetResponseCode.Approved)
                {
                    result.Success = true;

                    result.Amount = response.Amount;

                    result.AuthorizationCode = response.AuthorizationCode;

                    result.MerchantTransactionID = response.TransactionID;

                    result.Success = true;

                    return result;
                }

                //The transaction was not approved, find out why
                result.Success = false;

                var responseBase = response as GatewayResponse;

                //if (responseBase.CCVResponse != "M")
                //{
                //    result.Errors.Add("Invalid cvv number.");
                //}

                //if (responseBase.AVSResponse != "A")
                //{
                //    result.Errors.Add("Invalid billing address.");
                //}

                Int32 failureCode;

                if (Int32.TryParse(responseBase.RawResponse[2], out failureCode))
                {
                    result.ReasonCode = failureCode;
                }

                if (result.ResponseCode == AuthorizeNetResponseCode.Declined)
                {
                    result.Errors.Add("The credit card was declined.");

                    #region reasons
                    //if (responseBase.ReasonCode == 37)
                    //{
                    //    result.Errors.Add("Invalid credit card number.");
                    //}

                    ////38 Invalid merchant configuration

                    ////41 declined transaction fraudscreen.net

                    //if (responseBase.ReasonCode == 44)
                    //{
                    //    result.Errors.Add("Invalid cvv number");
                    //}

                    //if (responseBase.ReasonCode == 45)
                    //{
                    //    result.Errors.Add("There was a problem with the billing address or card code.");
                    //}

                    ////Fraud filter or block ip address
                    //if (responseBase.ReasonCode == 250 || responseBase.ReasonCode == 251)
                    //{
                    //    result.Errors.Add("Were sorry your payment cannot be processed at this time.");
                    //}

                    ////processor issued declines

                    //if (responseBase.ReasonCode == 315)
                    //{
                    //    result.Errors.Add("Invalid credit card number.");
                    //}

                    //if (responseBase.ReasonCode == 316)
                    //{
                    //    result.Errors.Add("Invalid credit card expiration date.");
                    //}

                    //if (responseBase.ReasonCode == 317)
                    //{
                    //    result.Errors.Add("The credit card has expired.");
                    //}

                    //if (responseBase.ReasonCode == 318)
                    //{
                    //    result.Errors.Add("A duplicate transaction has been submitted.");
                    //}
                    #endregion
                }
                else if (result.ResponseCode == AuthorizeNetResponseCode.Error)
                {
                    result.Errors.Add("There was a problem processing the credit card.");


                    #region reasons
                    //if (responseBase.ReasonCode == 6)
                    //{
                    //    result.Errors.Add("The credit card number is invalid.");
                    //}

                    //if (responseBase.ReasonCode == 7)
                    //{
                    //    result.Errors.Add("The credit card expiration date is invalid.");
                    //}

                    //if (responseBase.ReasonCode == 8)
                    //{
                    //    result.Errors.Add("The credit card has expired.");
                    //}

                    ////Duplicate transaction amount + card information withing specified time (+invoice number)
                    //if (responseBase.ReasonCode == 11)
                    //{
                    //    result.Errors.Add("A duplicate transaction has been detected.");
                    //}

                    //if (responseBase.ReasonCode == 13)
                    //{
                    //    throw new InvalidOperationException("Merchant account credentials are invalid.");
                    //}

                    //if (responseBase.ReasonCode == 17)
                    //{
                    //    result.Errors.Add("The credit card supplied is not supported by this merchant.");
                    //}

                    //if (responseBase.ReasonCode >= 19 && responseBase.ReasonCode <= 23 ||
                    //    responseBase.ReasonCode >= 25 && responseBase.ReasonCode <= 26)
                    //{
                    //    result.Errors.Add("An error occured while processing, please try again in 5 minutes.");
                    //}

                    //if (responseBase.ReasonCode == 27)
                    //{
                    //    result.Errors.Add("The billing address supplied does not match the card holders billing address.");
                    //}

                    //if (responseBase.ReasonCode == 28)
                    //{
                    //    result.Errors.Add("The credit card supplied is not supported by this merchant.");
                    //}

                    //if (responseBase.ReasonCode >= 57 && responseBase.ReasonCode <= 63)
                    //{
                    //    result.Errors.Add("An error occured while processing, please try again in 5 minutes.");
                    //}

                    ////29 30 31 34 35 merchant configuration errors

                    ////33 blank fields

                    //if (responseBase.ReasonCode == 78)
                    //{
                    //    result.Errors.Add("Invalid cvv number.");
                    //}
                    #endregion
                }
                else if (result.ResponseCode == AuthorizeNetResponseCode.UnderReview)
                {
                    result.Errors.Add("The credit card transaction could not be processed " +
                        ", the processor flagged the transaction for review. Please contact your card issuer and try again later.");
                }
                else
                    throw new InvalidOperationException("Invalid response code.");

                if (IsUserFriendlyAuthorizationMessage(result.ReasonCode))
                {
                    result.Errors.Add(response.Message);
                }

                return result;
            }
            catch (InvalidDataException dataEx)
            {
                //We most likely submitted invalid data to authorize.net
                //Log
                throw dataEx;
            }
            catch (WebException webEx)
            {
                //Log
                webEx.GetBaseException();

                result.Success = false;

                result.Errors.Add("A network error occured.");

                result.AuthorizeNetResponseMessage = "Could not contact the payment processor.";

                return result;
            }
        }

        public AuthorizeNetCaptureResult Capture(Decimal amount, String transactionID, String authCode)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException("amount", amount, "Amount must not be less than or equal to 0.");

            if (String.IsNullOrEmpty(authCode))
                throw new ArgumentNullException("authCode");

            if (String.IsNullOrEmpty(transactionID))
                throw new ArgumentNullException("transactionID");

            var result = new AuthorizeNetCaptureResult();

            try
            {

                var request = new CaptureRequest(amount, transactionID, authCode);

                var gateway = new Gateway(this._apiLoginID, this._transactionKey);

                var response = gateway.Send(request);

                if (response.Approved)
                {
                    result.Success = true;

                    return result;
                }

                result.Errors.Add(response.Message);

                result.Success = false;

                return result;
            }
            catch (WebException webEx)
            {
                webEx.GetType();

                result.Success = false;

                result.Errors.Add("A network error occured.");

                return result;
            }
        }

        private Boolean IsUserFriendlyAuthorizationMessage(Int32 reasonCode)
        {
            return _userFriendlyAuthorizationCodes.Contains(reasonCode);
        }

        private void popualteUserFriendlyAuthorizationCodes()
        {
            //11 Duplicate transaction 318 Duplicate transaction
            //128 please contact your card issuer
            //193 risk management transaction review
            //252 /253 accepted , merchant requested review

            _userFriendlyAuthorizationCodes = new List<Int32>()
                {6,7,8,17,19,20,21,22,23,25,26,27,28,37,41,44,45,57,58,59,60,61,62,63,65,78,120,121,122,127,128,145,165,170,171,172,173,174,175,180,181,250,251,261,289,315,316,317
                };

        }
    }

    public class AuthorizeNetCreditCardService : ICreditCardService
    {
        private AuthorizeNetService _authorizeNetService;

        private IUnitOfWorkForService _uow;

        public AuthorizeNetCreditCardService(IUnitOfWorkForService uow)
        {
            _authorizeNetService = new AuthorizeNetService();

            _uow = uow;
        }

        public CreditCardAuthorizationResult Authorize(CreditCardPayment payment, String description, Project.MVC.Entity.Models.Address billingAddress)
        {
            if (billingAddress == null)
                throw new ArgumentNullException("billingAddress");

            if (payment == null)
                throw new ArgumentNullException("payment");

            var result = new CreditCardAuthorizationResult();

            AuthorizeNetAuthorizationResult authResult = _authorizeNetService
                .Authorize(payment.CreditCardNumber, payment.Expiration.ToString("MM/yy"), payment.CVV, payment.Amount, description, billingAddress);

            if (authResult.Success)
            {
                result.Success = true;

                CreditCardTransaction transaction = new CreditCardTransaction()
                {
                    Amount = authResult.Amount,
                    AuthorizationCode = authResult.AuthorizationCode,
                    CardLastFour = payment.CreditCardNumber.Substring(payment.CreditCardNumber.Length - 4),
                    PaymentMethod = PaymentMethod.CreditCard,
                    TransactionDate = DateTime.Now,
                    TransactionType = TransactionType.Authorization
                };

                TransactionDetails transactionDetails = new TransactionDetails()
                {
                    TransactionProcessor = TransactionProcessor.AuthorizeNet,
                    TransactionProcessorTransactionID = authResult.MerchantTransactionID
                };

                transaction.TransactionDetails = transactionDetails;

                result.Transaction = transaction;

                return result;
            }

            result.Success = false;

            result.Errors.AddRange(authResult.Errors);

            return result;
        }

        public CreditCardCaptureResult Capture(CreditCardTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            var result = new CreditCardCaptureResult();

            var authNetCapRes = _authorizeNetService.Capture(transaction.Amount,
               transaction.TransactionDetails.TransactionProcessorTransactionID,
               transaction.AuthorizationCode);

            if (authNetCapRes.Success)
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;

                result.Errors.AddRange(authNetCapRes.Errors);
            }

            return result;
        }
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using UnityEngine;
using UnityEditor;

    /// <summary>
    /// See https://cloud.google.com/docs/authentication
    /// </summary>

    /// <summary>
    /// Configuration for connecting to a Google Sheet.
    /// </summary>
    public interface IGoogleSheetsService
    {
        /// <summary>
        /// The Google Sheet service that will be created using the Authorization API.
        /// </summary>
        SheetsService Service { get; }
    }

/// <summary>
/// The Sheets service provider performs the authentication to Google and keeps track of the authentication tokens
/// so that you do not need to authenticate each time.
/// The Sheets service provider also includes general sheet properties, such as default sheet styles, that are used when creating a new sheet.
/// </summary>
/// <example>
/// Unity recommends to have a <see cref="SheetsServiceProvider"/> asset pre-configured for use, however this example does create a new one.
/// <code source="../../../DocCodeSamples.Tests/GoogleSheetsSamples.cs" region="sheets-service-provider"/>
/// </example>
[CreateAssetMenu(fileName = "Google Sheets Service", menuName = "Data Migration/Google Sheets Service")]
[HelpURL("https://developers.google.com/sheets/api/guides/authorizing#AboutAuthorization")]
public class SheetsServiceProvider : ScriptableObject, IGoogleSheetsService, ISerializationCallbackReceiver
{

    [SerializeField]
    string clientID;
    [SerializeField]
    string clientSecret;

    [SerializeField]
    NewSheetProperties newSheetProperties = new NewSheetProperties();
    string applicationName;

    SheetsService sheetService;

    // The Google API access application we are requesting.
    static readonly string[] k_Scopes = { SheetsService.Scope.Spreadsheets };

    /// <summary>
    /// Used to make sure the access and refresh tokens persist. Uses a FileDataStore by default with "Library/Google/{name}" as the path.
    /// </summary>
    public IDataStore DataStore { get; set; }

    /// <summary>
    /// The Google Sheet service that will be created using the Authorization API.
    /// </summary>
    public virtual SheetsService Service
    {
        get
        {
            if (sheetService == null)
                sheetService = ConnectWithOAuth2();
            return sheetService;
        }
    }

    /// <summary>
    /// <para>Client Id when using OAuth authentication.</para>
    /// See also <seealso cref="SetOAuthCredentials"/>
    /// </summary>
    public string ClientId => clientID;

    /// <summary>
    /// <para>Client secret when using OAuth authentication.</para>
    /// See also <seealso cref="SetOAuthCredentials"/>
    /// </summary>
    public string ClientSecret => clientSecret;

    /// <summary>
    /// Properties to use when creating a new Google Spreadsheet sheet.
    /// </summary>
    public NewSheetProperties NewSheetProperties
    {
        get => newSheetProperties;
        set => newSheetProperties = value;
    }

    /// <summary>
    /// Enable OAuth 2.0 authentication and extract the <see cref="ClientId"/> and <see cref="ClientSecret"/> from the supplied json.
    /// </summary>
    /// <param name="credentialsJson"></param>
    public void SetOAuthCredentials(string credentialsJson)
    {
        var secrets = LoadSecrets(credentialsJson);
        clientID = secrets.ClientId;
        clientSecret = secrets.ClientSecret;
    }

    /// <summary>
    /// Enable OAuth 2.0 authentication with the provided client Id and client secret.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    public void SetOAuthCredentials(string clientId, string clientSecret)
    {
        clientID = clientId;
        this.clientSecret = clientSecret;
    }

    /// <summary>
    /// Call to preauthorize when using OAuth authorization. This will cause a browser to open a Google authorization
    /// page after which the token will be stored in IDataStore so that this does not need to be done each time.
    /// If this is not called then the first time <see cref="Service"/> is called it will be performed then.
    /// </summary>
    /// <returns></returns>
    public UserCredential AuthorizeOAuth()
    {
        // Prevents Unity locking up if the user canceled the auth request.
        // Auto cancel after 60 secs
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        var connectTask = AuthorizeOAuthAsync(cts.Token);
        if (!connectTask.IsCompleted)
            connectTask.RunSynchronously();

        if (connectTask.Status == TaskStatus.Faulted)
        {
            throw new Exception($"Failed to connect to Google Sheets.\n{connectTask.Exception}");
        }
        return connectTask.Result;
    }

    /// <summary>
    /// Call to preauthorize when using OAuth authorization. This will cause a browser to open a Google authorization
    /// page after which the token will be stored in IDataStore so that this does not need to be done each time.
    /// If this is not called then the first time <see cref="Service"/> is called it will be performed then.
    /// </summary>
    /// <param name="cancellationToken">Token that can be used to cancel the task prematurely.</param>
    /// <returns>The authorization Task that can be monitored.</returns>
    public Task<UserCredential> AuthorizeOAuthAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(ClientSecret))
            throw new Exception($"{nameof(ClientSecret)} is empty");

        if (string.IsNullOrEmpty(ClientId))
            throw new Exception($"{nameof(ClientId)} is empty");

        // We create a separate area for each so that multiple providers don't clash.
        var dataStore = DataStore ?? new FileDataStore($"Library/Google/{name}", true);

        var secrets = new ClientSecrets { ClientId = clientID, ClientSecret = clientSecret };

        // We use the client Id for the user so that we can generate a unique token file and prevent conflicts when using multiple OAuth authentications. (LOC-188)
        var user = clientID;
        var connectTask = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, k_Scopes, user, cancellationToken, dataStore);
        return connectTask;
    }

    /// <summary>
    /// When calling an API that will access private user data, O Auth 2.0 credentials must be used.
    /// </summary>
    public SheetsService ConnectWithOAuth2()
    {
        var userCredentials = AuthorizeOAuth();
        var sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = userCredentials,
            ApplicationName = applicationName,
        });
        return sheetsService;
    }

    public static ClientSecrets LoadSecrets(string credentials)
    {
        if (string.IsNullOrEmpty(credentials))
            throw new ArgumentException(nameof(credentials));
        using (var stream = new MemoryStream(System.Text.Encoding.ASCII.GetBytes(credentials)))
        {
            var gcs = GoogleClientSecrets.FromStream(stream);
            return gcs.Secrets;
        }
    }

     void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        if (string.IsNullOrEmpty(applicationName))
            applicationName = PlayerSettings.productName;
    }
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        
    }


}

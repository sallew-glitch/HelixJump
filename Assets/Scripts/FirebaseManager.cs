using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    long coins;

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("Profile")]
    public TMP_Text usernameText;

    async void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Prevents destruction on scene load
        }
        else 
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(gameObject);
            return;
        }

        //Check that all of the necessary dependencies for Firebase are present on the system
        dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            InitializeFirebase();
        }
        else
        {
            Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
        }

    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

        Debug.Log("FirebaseManager instance: " + instance.gameObject.name);

        if (auth.CurrentUser != null)
        {
            Debug.Log("User is still logged in: " + auth.CurrentUser.Email);
            StartCoroutine(GetUsernameFromDatabase());
        }
        else
        {
            Debug.Log("No user is logged in.");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIElements();  // This ensures UI elements are assigned after the scene loads
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Clean up event listener
    }

    public void AssignUIElements()
    {

        Transform uiParent = GameObject.Find("Canvas").transform;

        if (uiParent == null)
        {
            Debug.LogError("UI Parent is not assigned!");
            return;
        }

        Transform loginPanel = uiParent.Find("LoginPanel").transform;

        emailLoginField = loginPanel.Find("Login Email")?.GetComponent<TMP_InputField>();
        passwordLoginField = loginPanel.Find("Login Password")?.GetComponent<TMP_InputField>();
        warningLoginText = loginPanel.Find("Login Warning Text")?.GetComponent<TMP_Text>();
        confirmLoginText = loginPanel.Find("Login Warning Text").transform.Find("Login Confirm Text")?.GetComponent<TMP_Text>();

        Transform registerPanel = uiParent.Find("RegisterPanel").transform;

        usernameRegisterField = registerPanel.Find("Register Username")?.GetComponent<TMP_InputField>();
        emailRegisterField = registerPanel.Find("Register Email")?.GetComponent<TMP_InputField>();
        passwordRegisterField = registerPanel.Find("Register Password")?.GetComponent<TMP_InputField>();
        passwordRegisterVerifyField = registerPanel.Find("Register Confirm Password")?.GetComponent<TMP_InputField>();
        warningRegisterText = registerPanel.Find("Register Warning Text")?.GetComponent<TMP_Text>();

        Transform profilePanel = uiParent.Find("ProfilePanel").transform;

        usernameText = profilePanel.Find("Profile Username Text")?.GetComponent<TMP_Text>();

        Debug.Log("UI Elements Reassigned!");
    }


    private IEnumerator GetUsernameFromDatabase()
    {

        if (auth.CurrentUser == null)
        {
            Debug.LogWarning("User is null. Cannot fetch username.");
            usernameText.text = "Username : Not Logged In";
            yield break;
        }

        if (DBreference == null)
        {
            Debug.LogError("DBreference is null! Ensure Firebase Database is initialized.");
            yield break;
        }

        string userId = auth.CurrentUser.UserId;
        Debug.Log("Fetching username for User ID: " + userId);

        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(userId).Child("username").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to retrieve username: {DBTask.Exception}");
            usernameText.text = "Username : Error";
        }
        else if (DBTask.Result.Exists && DBTask.Result.Value != null)
        {
            string username = DBTask.Result.Value.ToString();
            usernameText.text = "Username : " + username;
        }
        else
        {
            Debug.LogWarning("Username not found in database.");
            usernameText.text = "Username : Not Set";
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            Debug.Log(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            Debug.Log("Firebase exception : " + firebaseEx.Message);
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            Debug.Log("Error : " + errorCode);

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";

            emailLoginField.text = "";
            passwordLoginField.text = "";

            UIManager.instance.Cross();
            StartCoroutine(GetUsernameFromDatabase());
            UIManager.instance.profileUI.SetActive(true);
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);
                        //Now return to login screen
                        usernameRegisterField.text = "";
                        emailRegisterField.text = "";
                        passwordRegisterField.text = "";
                        passwordRegisterVerifyField.text = "";
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

    public void LogoutButton()
    {
        if (auth != null)
        {
            auth.SignOut();
            Debug.Log("User logged out.");
        }

        UIManager.instance.Cross();
        confirmLoginText.text = "";
        usernameText.text = "";
    }

    public void UpdateCoins(int coins)
    {
        StartCoroutine(UpdateCoinsDatabase(coins));
    }

    private IEnumerator UpdateCoinsDatabase(int _coins)
    {
        if (User == null)
        {
            User = auth.CurrentUser;
            if (User == null)
            {
                Debug.LogError("User is still null. Cannot set coins.");
                yield break;
            }
        }

        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("coins").SetValueAsync(_coins);

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to update coins: {DBTask.Exception}");
        }
        else
        {
            Debug.Log("Coins successfully updated in the database.");
        }
    }

    public void GetCoins()
    {
        StartCoroutine(GetCoinsDatabase());
    }

    private IEnumerator GetCoinsDatabase()
    {
        while (auth == null)
        {
            Debug.LogWarning("Waiting for Firebase auth to initialize...");
            yield return null;
        }

        if (User == null)
        {
            User = auth.CurrentUser;
            if (User == null)
            {
                Debug.LogError("User is still null. Cannot retrieve coins.");
                yield break;
            }
        }

        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).Child("coins").GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to retrieve coins: {DBTask.Exception}");
        }
        else if (DBTask.Result.Exists)
        {
            coins = (long)DBTask.Result.Value;
            Debug.Log($"Coins: {coins}");
        }
        else
        {
            Debug.Log("No coins data found for the user.");
        }
    }

    public void GetLeaderboardData()
    {
        StartCoroutine(GetTop5Coins());
    }

    private IEnumerator GetTop5Coins()
    {
        while (auth == null)
        {
            Debug.LogWarning("Waiting for Firebase auth to initialize...");
            yield return null;
        }

        if (User == null)
        {
            User = auth.CurrentUser;
            if (User == null)
            {
                Debug.LogError("User is still null. Cannot retrieve leaderboard data.");
                yield break;
            }
        }

        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
            yield break;
        }

        Task<DataSnapshot> DBTask = DBreference.Child("users")
            .OrderByChild("coins")
            .LimitToLast(5) // Get top 5 users with the highest coins
            .GetValueAsync();

        yield return new WaitUntil(() => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning($"Failed to retrieve top 5 coins: {DBTask.Exception}");
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            List<(string username, long coins)> topPlayers = new List<(string, long)>();

            foreach (DataSnapshot user in snapshot.Children)
            {
                string userId = user.Key;
                long coins = user.Child("coins").Value != null ? (long)user.Child("coins").Value : 0;

                // Use stored username or fallback to DisplayName if available
                string username = user.Child("username").Value?.ToString();
                if (string.IsNullOrEmpty(username) && auth.CurrentUser != null && auth.CurrentUser.UserId == userId)
                {
                    username = auth.CurrentUser.DisplayName ?? "Unknown";
                }
                else
                {
                    username = username ?? "Unknown";
                }

                topPlayers.Add((username, coins));
            }

            // Sort in descending order (Firebase returns ascending by default)
            topPlayers.Sort((a, b) => b.coins.CompareTo(a.coins));

            Debug.Log("Top 5 collectors:");

            for (int i = 0; i < topPlayers.Count; i++)
            {
                if (i < gameManager.names.Count && i < gameManager.numCoins.Count) // Ensure no out-of-bounds error
                {
                    gameManager.names[i].text = (i + 1) + "    " + topPlayers[i].username;
                    gameManager.numCoins[i].text = topPlayers[i].coins.ToString();

                    // Get the parent GameObject and activate it
                    Transform parentTransform = gameManager.numCoins[i].transform.parent;
                    if (parentTransform != null)
                    {
                        parentTransform.gameObject.SetActive(true);
                    }

                }

                Debug.Log($"Username: {topPlayers[i].username}, Coins: {topPlayers[i].coins}");
            }
        }
    }

}
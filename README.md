# TcpInteract
TcpInteract is a simple, easy-to-use, TCP, .NET, networking library. It uses sockets directly (not any 3rd party service). No concern is needed for parsing/serializing data or implementing asynchronous logic. TcpInteract has a server-side client and a client-side client which both greatly abstract from using sockets directly. It has a simple client system where the clients connect, then send a login request. The request is then accepted or rejected by the server. The client sends a login package which contains its name. Names are used to distinguish clients – not socket handles.

![alt text](http://i.imgur.com/jbbGbp4.png "Server-client communication")

The server will reject a client that tries to login with the same name as another client. It also has a logout, kick, and server closed notification that can all disconnect clients gracefully. TcpInteract provides both synchronous and asynchronous methods but does not make great use of Tasks. In particular, the system marshals asynchronous operations onto the UI thread automatically with SynchronizationContext. If no context is provided to the server or client-side client, the current SynchronizationContext is used (SynchronizationContext.Current). Current is null up until UI is created, make sure to construct any server or client-side client implementations after the UI sync context is available (this applies to WinForms and likely WPF). Other than this, no concern for thread-safety is needed.
TcpInteract uses events for notifications which do not correspond to any specific type of received data. However, for received data, both the ServerBase and ClientBase use a Type-Action binding system, allowing the consumer to easily publish occurrences of received data, or subscribe to datatypes coming in through one of the base classes. There is no need to define events for each kind of data received.
The publisher is already defined in ServerBase and ClientSideClient:

``` C#
public ContentPusher Pusher { get; } = new ContentPusher();
```

Push to publisher:

``` C#
Pusher.Push(InstantMessageContent.Deserialize(package.Content));
```

Subscribe:

``` C#
Pusher.Bind<LoginContent>(e => SubmitLog($@"{e.ClientName} has logged in."));
```

Or

``` C#
Pusher.Bind((LoginContent e) => SubmitLog($@"{e.ClientName} has logged in."));
```

Unsubscribe:

``` C#
Pusher.Unbind<LoginContent>(ProcessLoginContent);
```

In essence, no publisher needs to be defined, the publisher is the “Pusher” property defined in both the ServerBase and ClientSideClient classes. The publisher was initially a singleton but this would introduce problems when the server is implemented in the same application as the client-side client. 

## The Projects
The solution consists of six projects, each targeting .NET Framework 4.6:

    TcpInteract: The main project, having a fairly flat namespace.
    DemoClient: A demonstration client, currently an instant messenger application The demo clients send and receive text messages, screenshots, and the users cursor position. Connects to port 2059.
    DemoServer: A command line interface to connect DemoClient applications. Binds to port 2059.
    DemoShared: Provides common types and commands to both the DemoServer and DemoShared project.
    UnitTesting: Provides unit testing for various things.
    TcpInteract.Forms: Provides WinForms helpers. It provides a login Form, which is used by the demo client, and a console command Form, which is used by the server demo.

The demo server has a console command system that is enabled via reflections. Commands can be added by simply declaring a method with a name that ends in "Cmd". The command system will automatically identify the method as a command. The start of the method name, without the "Cmd", is the command's key. Any text that follows is the command's argument. Command methods can be defined with a single string parameter and/or no parameters. A description attribute can be added to the command method so that the "help" command can output what the command is for.

``` C#
[Description("Sends a goodbye message to the clients, then restarts the server.")]
private void RestartCmd(string message)
{
    StopCmd(message);
    StartCmd();
}

[Description("Restarts the server without sending a message.")]
private void RestartCmd()
{
    RestartCmd(null);
}
```

In this case, "restart" is what needs to be typed initially, then optional text afterwards (to be handled by the command method). Note, Resharper will ask to introduce optional parameters, but optional parameters will cause the command system to behave unexpectedly.

Type help to see what each preset command does.

## The Shared Library
To properly use TcpInteract, create a class library with shared types and commands (like the DemoShared project). This allows 3rd party types to be easily serialized and deserialized in both the client and server. It also allows the commands used by the server and client to be consistent. Of course, the server and client implementations can be in the same project, but this is usually not the case. TcpInteract uses built-in binary serialization for its predefined content datatypes (datatypes such as LoginContent which provides a login notification). Note, “content datatypes” will refer to datatypes that instantiate data that is sent from the client to the server and vice versa. The library provides abstractions to use the same serialization with user-defined datatypes. The base content datatypes names are postfixed with the text “Content”, for consistency, considering doing the same with consumer datatypes. Begin by defining datatypes in a shared library. To fully leverage the abstraction, derive the content types from Serializable<T> and mark the type as serializable.

```C#
/// <summary>
/// Represents a message of an instant messaging session.
/// </summary>
[Serializable]
public class InstantMessageContent : SerializableContent<InstantMessageContent>…
```

SerializableContent inherits ISerializable, and thus allows the more abstracting “Send” methods to be used:

```C#
/// <summary>
/// Sends the specified package asynchronously to the server.
/// </summary>
public void SendPackageAsync(int command, ISerializable serializable)
{
    SendPackageAsync(new Package(command, serializable.Serialize()));
}
```

The serializer used does not require properties to have a public setter, which is great since Content types are “packages” of data which are meant to be prepared, sent, and read on the receiver side. By no means should the state of a content instance change. TcpInteract does not manage content routing automatically. If content needs to be routed to a specified client or set of clients, add a string or string array to the content type to represent the destination(s) of the package.

``` C#
[Serializable]
public class InstantMessageContent : SerializableContent<InstantMessageContent>
{
    public string Message { get; }
    public string SenderName { get; }

    public InstantMessageContent(string message, string senderName)
    {
        Message = message;
        SenderName = senderName;
    }

    public override string ToString()
    {
        return $"{SenderName}: {Message}";
    }
}
```

TcpInteract uses a basic command system to identify what package is being received. Commands are 32-bit integers when they are sent but are best defined with enumerations. Enumerations allow the developer to defined commands without having to worry about numbering each possible value. TcpInteracts base commands are all negative values, so there is no need to worry about conflicting commands. Simply create an enum value for each Content type defined. Commands will mostly correspond to a content datatype, but it is perfectly fine to have commands that are stand-alone – perhaps for an update notification or request for data.

``` C#
/// <summary>
/// Represents commands for sending and receiving data packages.
/// </summary>
public enum Commands
{
    /// <summary>
    /// Client To Client: Indicates the contents of the corresponding Package is an <see cref="InstantMessageContent"/>.
    /// </summary>
    InstantMessage,
    /// <summary>
    /// Client To Client: Indicates the contents of the corresponding package is a <see cref="ScreenshotContent"/>.
    /// </summary>
    Screenshot,
    /// <summary>
    /// Client To Clients: Indicates the contents of the corresponding package is <see cref="ClientCursorContent"/>.
    /// </summary>
    CursorPosition
}
```
## Implementing a Server
Considering this library is light-weight and is, thus, for light-weight projects, it may be desirable to package both the server and client in a single, intuitive application. However, the library hasn’t been tested in such a scenario.
To setup the server:

1. Add a reference to both TcpInteract and the user-defined shared library (created earlier).
2. Derive from ServerBase.
3.	Implement OnPackageReceived. This methods is invoked on the UI thread. It handles a Package type. Packages are mostly unserialized content – they are merely commands bound to byte arrays. Only the command and the chunk of data it represents are deserialized, as this is all that is needed to identify what to do with the data. As well, fully deserializing all packages on the server side can be inefficient; this is especially apparent in a file transfer system.

![alt text](http://i.imgur.com/dvwcOgK.png "Packages")

4. A constructor with a single integer parameter also needs to be defined. Pass in a port that is not already in use).
5. Override the Synchronize method. This method will be called when a specific client asks for synchronization content. The base implementation of Synchronize sends a list of client names to the syncing client. The below example, adds of to this base implementation by sending instant message history tracked by the server.

``` C#
public sealed class MessengerServer : ServerBase
{
    /// <summary>
    /// The message history for this session.
    /// </summary>
    private readonly List<InstantMessageContent> messages = new List<InstantMessageContent>();

    public MessengerServer(int port) : base(port) { }

    protected override void Synchronize(ServerSideClient client)
    {
        base.Synchronize(client);

        foreach (var message in messages)
            client.SendPackageAsync((int)Commands.InstantMessage, message);
    }

    protected override void OnPackageReceived(ServerSideClient client, Package package)
    {
        switch ((Commands)package.Command)
        {
            // Broadcast messages to all clients.
            case Commands.InstantMessage:
                var message = InstantMessageContent.Deserialize(package.Content);
                messages.Add(message);
                Pusher.Push(message);
                BroadcastPackageAsync(package);
                break;

            // Send screenshot specific client.
            case Commands.Screenshot:
                var screenshot = ScreenshotContent.Deserialize(package.Content);
                ClientBase destClient = Clients.FirstOrDefault(c => c.Name == screenshot.RecieverName);
                destClient?.SendPackageAsync(package);
                break;

               // Send cursor to all clients. Does not need to be deserialized because
               // the contents of the package do not need to be analyzed.
            case Commands.CursorPosition:
                BroadcastPackageAsync(package);
                break;
        }
    }
}
```

In this case, and in most, the derived server is a type that will never be derived from – consider marking it as sealed. Data can be broadcasted to all connected clients using the “Broadcast” methods. By default, these methods broadcast data to logged-in clients only. To send data to a specific client, simply find the client by name in the Clients list and use one of the client's SendPackage methods. Calls made in rapid succession to SendPackageAsync will queue data correctly unless calls are being made in rapid succession from various threads simultaneously.
## Implementing a Client-side Client
Unlike the server-side client, the client-side client is meant to be derived from. Derive from the ClientSideClient class. Create abstracting Send methods much like the existing ones, only suited to specific send operations or types. Deserialize and push received packages, which are to be processed elsewhere, to the content publisher.

``` C#
/// <summary>
/// Sends an instant message asynchronously.
/// </summary>
public void SendMessageAsync(string message)
{
    SendPackageAsync((int)Commands.InstantMessage, new InstantMessageContent(message, Name));
}
```

Override OnPackageReceived and process received packages by their command. Push content into the publisher to be read by consumer code.

``` C#
protected override void OnPackageReceived(Package package)
{
    switch ((Commands)package.Command)
    {
        case Commands.InstantMessage:
            Pusher.Push(InstantMessageContent.Deserialize(package.Content));
            break;

        case Commands.Screenshot:
            Pusher.Push(ScreenshotContent.Deserialize(package.Content));
            break;
    }
}
```

## Consuming the Server Implementation
The demo server in the solution will be used as an example here. The demo server consumes the server implementation directly, however, the demo code was implemented this way for simplicity. A scalable application should not directly consume the server in this manner, the views should not have a great degree of code-backing. To begin, declare the server implementation, then initialize it when the desired sync context becomes available.

``` C#
private readonly MessengerServer server;

public MainForm()
{
    InitializeComponent();
    server = new MessengerServer(2059);
    …
}
```

Make sure the parent class implements IDisposable and is able to properly dispose of the server. In a Windows Form, dispose of the server in the Dispose override. This method runs when the both the Form.Close() and Application.Exit() are is used. Whereas OnClosing is not invoked when Application.Exit() is called.

``` C#
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        server.Dispose();
    }
    if (disposing && (components != null))
    {
        components.Dispose();
    }
    base.Dispose(disposing);
}
```

Subscribe to content notifications by using the server’s ContentPusher. The following subscriptions display received content in a listbox:

``` C#
public MainForm()
{
    InitializeComponent();
    …
    server.Pusher.Bind<InstantMessageContent>(e => SubmitLog(e.ToString()));
    server.Pusher.Bind<LoginContent>(e => SubmitLog($@"{e.ClientName} has logged in."));
    server.Pusher.Bind<LogoutContent>(e => SubmitLog($@"{e.ClientName} has logged out. Reason: {e.Reason}."));
    server.Pusher.Bind<ConnectionRefusedContent>(SubmitConnectionRefusedContent);
}
private void SubmitConnectionRefusedContent(ConnectionRefusedContent content)
{
    SubmitLog($@"{content.ClientName} has been refused, reasons: {content.Reason}.");
}
```

The remote or public IP used to connect to the server can be retrieved like so:
``` C#
protected override async void OnLoad(EventArgs e)
{
    textBoxIP.Text = await Networking.GetPublicIpStringTaskAsync();
}
```
Starting and stopping the server is quite simple. Call the Start and Stop() methods. Call Stop(String) to pass a “server closed” message to the clients along with the stop command.

``` C#
private void buttonEnabled_CheckedChanged(object sender, EventArgs e)
{
    if (buttonEnabled.Checked)
    {
        try
        {
            server.Start();
            SubmitLog("Server started.");
        }
        catch (SocketException ex)
        {
            SubmitLog(ex.Message);
            // Setting button enabled to false below will raise this handler again,
            // failedStart will the else condition of this if from firing unnecessarily.
            failedStart = true;
            buttonEnabled.Checked = false;
            failedStart = false;
        }
    }
    else if (!failedStart)
    {
        server.Stop();
        SubmitLog("Server stopped.");
    }
}
```

The server does not expose the connected clients directly but does provide a bindable list of the names of the logged-in clients.

``` C#
listUsers.DataSource = server.ClientNames;
```

If information is needed about a client, simple use ServerBase.GetClientInfo(string). This, of course, will return significant, read-only information about the client.

``` C#
/// <summary>
/// Gets the key information about the specified client.
/// </summary>
/// <param name="clientName">The name of the client.</param>
/// <returns>Null, if no client exists with the specified name.</returns>
public ClientInfo GetClientInfo(string clientName)
{
    return ClientInfo.FromClient(clients.FirstOrDefault(c => c.Name == clientName));
}
```

## Consuming the Client Implementation
The client demo project code will be used for this section. The client demo has two Forms, one for login and one chat Form. Both of the Forms are passive and are controlled by the AppContext class. To begin, declare the client implementation and initialize it. Initialize it after the current SynchronizationContext is made available. In the demo, the following will work:

``` C#
private readonly MessengerForm formMessenger = new MessengerForm();
private readonly LoginForm formLogin = new LoginForm();
private readonly MessengerClient client = new MessengerClient();
```

But this raises an exception:

``` C#
private readonly MessengerClient client = new MessengerClient();
private readonly MessengerForm formMessenger = new MessengerForm();
private readonly LoginForm formLogin = new LoginForm();
```

Subscribe to client events and content pushes:

``` C#
client.ConnectionAttemptFailed += (s, e) => formLogin.Status = "Attempt #" + client.ConnectionAttempts;
client.StatusChanged += OnStatusChanged;

client.Pusher.Bind<ServerClosedContent>(content =>
{
    MessageBox.Show("The server has closed.", Application.ProductName,
        MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
    formMessenger.Close();
});
client.Pusher.Bind<ConnectionRefusedContent>(ClientOnConnectionRefused);
client.Pusher.Bind<LoginContent>(e => formMessenger.SubmitMessage($@"{e.ClientName}: has logged in."));
client.Pusher.Bind<LogoutContent>(ClientOnClientLoggedOut);
client.Pusher.Bind<InstantMessageContent>(e => formMessenger.SubmitMessage($@"{e.SenderName}: {e.Message}"));
client.Pusher.Bind<ScreenshotContent>(screenshot => formMessenger.Screenshot = (Bitmap)screenshot.Image);
```

Specifically, the ClientStatusChanged event should be used to see if the client has logged in. The status of a client can be: logged in, connecting, connected (but not logged in), or disconnected (idle). All situations where the client can be disconnected should immediately reflect in the StatusChanged property. So hooking to this event will guarantee that the most recent client state is reflected in the user experience. For instance, the client status will be set to disconnected when the client has logged out, the server has closed, or the server has kicked the client.

``` C#
private void ClientStatusChanged(object sender, EventArgs e)
{
    switch (client.Status)
    {
        case ClientStatus.Connected:
            formLogin.Status = "Connected. Awaiting login approval.";
            formLogin.LoggingIn = true;
            break;

        case ClientStatus.Disconnected:
            formLogin.Status = "Idle.";
            formLogin.LoggingIn = false;
            break;

        case ClientStatus.LoggedIn:
            formLogin.Status = "Logged in.";
            formLogin.LoggingIn = false;
            formLogin.Hide();
            formMessenger.SetClientName(client.Name);
            formMessenger.Show();
            client.Synchronize();
            formMessenger.SetDebugInfo(ClientInfo.FromClient(client).GetLines());
            break;
    }
}
```

Note, the logged in case hides the login Form and shows the main Form. It also calls Synchronize(), which will ask for a list of client names, and in the case of the demo, instant message history. To assess the logout reason, bind to the LogoutContent type. This type has a “Reason” property which specifies roughly how the client has disconnected. There are currently three possible values for this property:

1. Kicked. The client has been kicked by the server.
2. TimedOut. The client has abruptly lost connection to the server.
3. UserSpecified. The client was disconnected gracefully, sending a logout message to the server.

Be sure to check kick notifications to see if they pertain to the local client.

``` C#
private void ClientOnClientLoggedOut(LogoutContent content)
{
    string message;

    switch (content.Reason)
    {
        case LogoutReason.Kicked:
            message = $@"{content.ClientName}: was kicked. Reason: {content.Message}";

            if (content.ClientName == client.Name)
            {
                …
                formMessenger.Close();
            }
            break;

        case LogoutReason.TimedOut:
            message = $@"{content.ClientName}: timed out.";
            break;

        case LogoutReason.UserSpecified:
            message = $@"{content.ClientName}: logged out.";
            break;

        default:  throw new InvalidEnumArgumentException();
    }

    formMessenger.SubmitMessage(message);
}
```

Send packages by calling send methods of the client defined in the client implementation.

``` C#
using (Bitmap capture = CaptureScreen(Screen.GetBounds(formMessenger.Bounds)))
    client.SendScreenAsync(capture, clientName);
```

Just like the ServerBase class, the ClientSideClient exposes a bindable list of client names. Use it to display a list of logged in clients.

``` C#
private readonly BindingList<string> clientNames = new BindingList<string>();
/// <summary>
/// Gets a bindable list of client names.
/// </summary>
public IReadOnlyList<string> ClientNames => clientNames;
```

Now that the consumer is setup for logging in. It is time to login the client implementation. Start by setting the name and IPEndPoint of the client.

``` C#
IPAddress address;

try
{
    address = IPAddress.Parse(formLogin.Address);
}
catch (FormatException)
{
    ShowErrorMessage("Invalid address format.");
    return;
}

client.Name = formLogin.ClientName;
client.EndPoint = new IPEndPoint(address, PORT);
```

Then call RequestLogin(), which asks the server to login. Unlike Logout(), Login() does not guarantee the desired result.

``` C#
try
{
    client.RequestLogin();
}
catch (InvalidOperationException ex)
{
    formLogin.Status = ex.Message;
    formLogin.LoggingIn = false;
}
catch (AlreadyLoggedInException ex)
{
    formLogin.Status = ex.Message;
    formLogin.LoggingIn = false;
}
```

When the login is approved, the Status property of the client changes to LoggedIn, where the UI is then adjusted appropriately.
Logins can be refused by the server for several reasons.

``` C#
/// <summary>
/// Describes the possible reasons why a connection may be refused.
/// </summary>
[Flags]
public enum ConnectionRefusedReason
{
    /// <summary>
    /// No reason.
    /// </summary>
    None,
    /// <summary>
    /// Name is null, empty, or whitespace.
    /// </summary>
    EmptyName,
    /// <summary>
    /// A logged in client already has that name.
    /// </summary>
    NameExists,
    /// <summary>
    /// The client name has been invalidated by a regex pattern.
    /// </summary>
    RegexInvalidated,
    /// <summary>
    /// The client connected but did not log in soon enough.
    /// </summary>
    NoLogin
}
```

Handle this potential outcome with a Pusher binding.

``` C#
private void ClientOnConnectionRefused(ConnectionRefusedContent e)
{
    formLogin.Status = "Connection refused: " + e.Reason;
    formLogin.LoggingIn = false;
}
```

Be sure to dispose of the client when done with it.

``` C#
protected override void ExitThreadCore()
{
    client.Dispose();
    base.ExitThreadCore();
}
```

## Server Features
The server automatically refuses clients when they try to log in with a name that is already being used. This is quite important since the server distinguishs the clients by their name. The server also rejects null or empty names. The ServerBase.RefusePattern property is a regex string that can be set to further validate the client names. Pattern matches reject clients or invalidates otherwise valid logins.
The server has a client poller. The client poller disconnects clients that are connected but not logged in. The clients must be in this state for three seconds, by default, before they are disconnected. The poller also checks for clients that have timed out. Clients that have timed out have not sent a logout notification, therefore the server is unaware of the client’s state until the server tries to send data to it. The poller checks for disconnected clients, by default, every five seconds. Once a disconnected client is found, it is removed from the client list and a logout notification is sent to all logged in clients. The PollWait, SolicitorCheckInterval, and SolicitorThreshold properties can be set to customize the poller’s strictness.
ServerBase has a kick method – KickClient(string, string). Pass in the name of the client to be kicked and an optional message. All logged in clients will be notified of the kick operation. Note, the LogoutContent facilitates notifications for kicks, typical logouts, and timed out clients.

``` C#
var args = new LogoutContent(name, LogoutReason.Kicked, reason);
var package = new Package((int)BaseCommands.Logout, args.Serialize());
BroadcastPackage(package);
```

## Client Features
RequestLogin() continuously attempts to connect until AbortConnect() is called or the max connection attempts has been reached.  The MaxConnectionAttempts property can be set to limit how many automatic connection attempts are made after calling RequestLogin(). By default this property is set to zero. Zero indicates that the client should try to connect indefinitely.
Clients can be manually polled. That is, they can be checked for connectivity (since Socket.Connected only reflects the last known state of the Socket). Use the IsInactive methods to see if a client is still connected.

``` C#
public Task<bool> IsInactiveTaskAsync(int waitTime);
public bool IsInactive(int waitTime);
```

The connection time and login time are automatically tracked for both the server-side client and the client-side client.

``` C#
/// <summary>
/// Gets the time in which this client has established a connection. 
/// </summary>
public DateTime? ConnectionTime { get; protected set; }

/// <summary>
/// Gets the time in which this client has logged in. 
/// </summary>
public DateTime? LoggedInTime { get; protected set; }
```

## Potential Improvements
1.	It may be best to identify the clients by the socket handle instead of the client’s name.
2.	It may be possible to overload the plus and minus operators in the ContentPusher class, to provide more elegant subscriptions (-=, +=). So far I have not found a way to do this.
3.	Built-in file transferring would be nice.
4.	Add a port checking tool?
5.	More bindable properties.
6. Consider an MVP implementation to be used for the client demo for unit testing.

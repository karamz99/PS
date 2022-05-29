using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using PS.Core;
using PS.Core.EventArguments;
using PS.Core.Exceptions;
using PS.Core.Helpers;
using PS.Core.Http;
using PS.Core.Models;
using PS.Core.Network;
using PS.Core.StreamExtended.Network;
using PS.Data;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PS.Services
{
    public class ProxyService : IHostedService
    {
        private readonly ILogger<ProxyService> logger;
        private readonly ProxyServer proxyServer;
        private readonly MainContext context;
        private readonly Dictionary<HttpWebClient, SessionListItem> sessionDictionary = new Dictionary<HttpWebClient, SessionListItem>();
        private int lastSessionNumber;

        public static List<EventCallback> SessionsChanged { get; set; } = new List<EventCallback>();
        public static List<Customer> Customers { get; set; }
        public static ObservableCollectionEx<SessionListItem> Sessions { get; } = new ObservableCollectionEx<SessionListItem>();
        public static string[] BlackList { get; set; }


        public ProxyService(ILogger<ProxyService> logger)
        {
            this.logger = logger;
            this.context = new MainContext();
            this.proxyServer = new ProxyServer();

            proxyServer.EnableHttp2 = true;

            proxyServer.CertificateManager.CertificateEngine = CertificateEngine.DefaultWindows;

            ////Set a password for the .pfx file
            //proxyServer.CertificateManager.PfxPassword = "PfxPassword";

            ////Set Name(path) of the Root certificate file
            //proxyServer.CertificateManager.PfxFilePath = @"C:\NameFolder\rootCert.pfx";

            ////do you want Replace an existing Root certificate file(.pfx) if password is incorrect(RootCertificate=null)?  yes====>true
            //proxyServer.CertificateManager.OverwritePfxFile = true;

            ////save all fake certificates in folder "crts"(will be created in proxy dll directory)
            ////if create new Root certificate file(.pfx) ====> delete folder "crts"
            //proxyServer.CertificateManager.SaveFakeCertificates = true;

            //var RootStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            //RootStore.Open(OpenFlags.MaxAllowed);
            //X509Certificate2Collection certificates = RootStore.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificate.Subject, false);
            //RootStore.RemoveRange(certificates);
            //RootStore.Add(certificate);
            //RootStore.Close();

            proxyServer.ForwardToUpstreamGateway = true;

            //increase the ThreadPool (for server prod)
            proxyServer.ThreadPoolWorkerThread = Environment.ProcessorCount * 6;

            ////if you need Load or Create Certificate now. ////// "true" if you need Enable===> Trust the RootCertificate used by this proxy server
            //proxyServer.CertificateManager.EnsureRootCertificate(true, true);

            ////or load directly certificate(As Administrator if need this)
            ////and At the same time chose path and password
            ////if password is incorrect and (overwriteRootCert=true)(RootCertificate=null) ====> replace an existing .pfx file
            ////note : load now (if existed)
            //proxyServer.CertificateManager.LoadRootCertificate(@"C:\NameFolder\rootCert.pfx", "PfxPassword");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            BlackList = await context.BlackLists.OrderBy(x => x.Url).Select(x => x.Url).ToArrayAsync();
            Customers = await context.Customers.Include(x => x.Group).ToListAsync();

            //proxyServer.EnableWinAuth = true;
            //basic, NTLM, Kerberos, Negotiate
            //proxyServer.ProxyAuthenticationSchemes = ProxyAuthenticationSchemeList.Basic;
            //proxyServer.ProxySchemeAuthenticateFunc = async (session, username, password) =>
            //{
            //    Console.WriteLine("=====================================");
            //    Console.WriteLine(session.ClientUserData);
            //    Console.WriteLine($"Username: {username}, Password: {password}");
            //    if (session.ClientUserData is DateTime && DateTime.Now.AddMinutes(5).CompareTo((DateTime)session.ClientUserData) < 0)
            //    {
            //        Console.WriteLine("Logged: " + DateTime.Now);
            //        session.ClientUserData = DateTime.Now;
            //        return new ProxyAuthenticationContext { Result = ProxyAuthenticationResult.Success };
            //    }
            //    else
            //    {
            //        Console.WriteLine("Need Userser & Password");
            //        var logCustomer = await context.Customers.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
            //        if (logCustomer != null)
            //        {
            //            Console.WriteLine("Valid user & pass");
            //            session.ClientUserData = DateTime.Now;
            //            logCustomer.IP = session.ClientRemoteEndPoint.Address.ToString();
            //            logCustomer.LastRequestTime = DateTime.Now;
            //            await context.SaveChangesAsync();
            //            return new ProxyAuthenticationContext { Result = ProxyAuthenticationResult.Success };
            //        }
            //        Console.WriteLine("InValid user & pass");
            //        return new ProxyAuthenticationContext { Result = ProxyAuthenticationResult.Failure };
            //    }
            //};

            var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, 8000);

            //proxyServer.ProxySchemeAuthenticateFunc = async (session, username, password) =>
            //{
            //    Console.WriteLine($"Username: {username}, Password: {password}, Date: {DateTime.Now}");
            //    var customer = Customers.FirstOrDefault(x => x.Username == username && x.Password == password);
            //    if (customer != null)
            //    {
            //        if (customer.IP != session.ClientRemoteEndPoint.Address.ToString() || customer.LastRequestTime < DateTime.Now.AddMinutes(5))
            //        {
            //            customer.IP = session.ClientRemoteEndPoint.Address.ToString();
            //            customer.LastRequestTime = DateTime.Now;
            //            await context.SaveChangesAsync();
            //        }
            //        return new ProxyAuthenticationContext { Result = ProxyAuthenticationResult.Success };
            //    }
            //    return new ProxyAuthenticationContext { Result = ProxyAuthenticationResult.Success };
            //};

            proxyServer.ProxyBasicAuthenticateFunc = async (session, username, password) =>
            {
                var customer = Customers.FirstOrDefault(x => x.Username == username);
                if (customer != null && customer.CheckPassword(password))
                {
                    if (customer.IP != session.ClientRemoteEndPoint.Address.ToString())
                    {
                        customer.IP = session.ClientRemoteEndPoint.Address.ToString();
                        context.SaveChanges();
                    }
                    return await Task.FromResult(true);
                }
                return await Task.FromResult(false);
            };

            proxyServer.AddEndPoint(explicitEndPoint);
            //proxyServer.UpStreamHttpProxy = new ExternalProxy
            //{
            //    HostName = "158.69.115.45",
            //    Port = 3128,
            //    UserName = "Titanium",
            //    Password = "Titanium",
            //};

            //var socksEndPoint = new SocksProxyEndPoint(IPAddress.Any, 1080, true)
            //{
            //    // Generic Certificate hostname to use
            //    // When SNI is disabled by client
            //    //GenericCertificateName = "google.com"
            //};

            //proxyServer.AddEndPoint(socksEndPoint);

            proxyServer.BeforeRequest += ProxyServer_BeforeRequest;
            proxyServer.BeforeResponse += ProxyServer_BeforeResponse;
            proxyServer.AfterResponse += ProxyServer_AfterResponse;
            explicitEndPoint.BeforeTunnelConnectRequest += ProxyServer_BeforeTunnelConnectRequest;
            explicitEndPoint.BeforeTunnelConnectResponse += ProxyServer_BeforeTunnelConnectResponse;

            proxyServer.ClientConnectionCountChanged += delegate { clientConnectionCount = proxyServer.ClientConnectionCount; };
            proxyServer.ServerConnectionCountChanged += delegate { serverConnectionCount = proxyServer.ServerConnectionCount; };

            proxyServer.Start();

            proxyServer.SetAsSystemProxy(explicitEndPoint, ProxyProtocolType.AllHttp);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            proxyServer.BeforeRequest -= ProxyServer_BeforeRequest;
            proxyServer.BeforeResponse -= ProxyServer_BeforeResponse;
            proxyServer.AfterResponse -= ProxyServer_AfterResponse;
            proxyServer.ClientConnectionCountChanged -= delegate { clientConnectionCount = proxyServer.ClientConnectionCount; };
            proxyServer.ServerConnectionCountChanged -= delegate { serverConnectionCount = proxyServer.ServerConnectionCount; };
            proxyServer.Stop();
        }

        DateTime lastSavedTime;
        private async Task SaveChanges()
        {
            if (DateTime.Now.CompareTo(lastSavedTime.AddSeconds(5)) > 0)
            {
                lastSavedTime = DateTime.Now.AddSeconds(5);
                var oldCustomers = await context.Customers.Where(x => x.LastRequestTime.HasValue && DateTime.Now.AddMinutes(5).CompareTo(x.LastRequestTime.Value) > 1).ToListAsync();
                oldCustomers.ForEach(x => { x.LastRequestTime = null; x.IP = null; });
                var i = await context.SaveChangesAsync();
                Console.WriteLine($"{lastSavedTime} - {i}");
            }
        }

        private SessionListItem selectedSession;
        public SessionListItem SelectedSession
        {
            get => selectedSession;
            set
            {
                if (value != selectedSession)
                {
                    selectedSession = value;
                    selectedSessionChanged();
                }
            }
        }

        private int clientConnectionCount;
        public int ClientConnectionCount
        {
            get => clientConnectionCount;
            set => clientConnectionCount = value;
        }

        private int serverConnectionCount;
        public int ServerConnectionCount
        {
            get => serverConnectionCount;
            set => serverConnectionCount = value;
        }

        private async Task ProxyServer_BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs e)
        {
            string hostname = e.HttpClient.Request.RequestUri.Host;
            if (hostname.EndsWith("webex.com"))
            {
                e.DecryptSsl = false;
            }

            await Task.Run(async () => { await addSession(e); });
        }

        private async Task ProxyServer_BeforeTunnelConnectResponse(object sender, TunnelConnectSessionEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (sessionDictionary.TryGetValue(e.HttpClient, out var item))
                    {
                        item.Update(e);
                    }
                }
                catch { }
            });
        }

        private async Task ProxyServer_BeforeRequest(object sender, SessionEventArgs e)
        {
            await SaveChanges();
            //if (e.HttpClient.Request.HttpVersion.Major != 2) return;

            // To cancel a request with a custom HTML content
            // Filter URL
            if (BlackList.Any(x => e.HttpClient.Request.RequestUri.AbsoluteUri.ToLower().Contains(x)))
                e.Ok("<!DOCTYPE html>" +
                    "<html><body><h1>" +
                    "Website Blocked" +
                    "</h1>" +
                    "<p>Blocked by proxy server.</p>" +
                    "</body>" +
                    "</html>");

            // Redirect example
            if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains("wikipedia.org"))
                e.Redirect("https://www.paypal.com");

            SessionListItem item = null;
            await Task.Run(async () => { item = await addSession(e); });

            if (e.HttpClient.Request.HasBody)
            {
                e.HttpClient.Request.KeepBody = true;
                await e.GetRequestBody();

                if (item == SelectedSession)
                    await Task.Run(selectedSessionChanged);
            }

            var cust = Customers.ByIP(e.ClientRemoteEndPoint.Address);
            if (cust != null && cust.Group?.DataUsageControl == true && cust.DataUsage > cust.Group?.DataUsageInBytes)
            {
                e.Ok("<!DOCTYPE html>" +
                    "<html><body><h1>" +
                    "Data usage finished" +
                    "</h1>" +
                    "<p style='color:red;'>Blocked by proxy server because data usage " +
                    $"[{Customer.FormatUsage(cust.DataUsage)}/{Customer.FormatUsage(cust.Group.DataUsageInBytes)}] is finished.</p>" +
                    "</body>" +
                    "</html>");
                return;
            }

            if (cust != null && cust.Group?.InValidTime == true)
            {
                e.Ok("<!DOCTYPE html>" +
                    "<html><body><h1>" +
                    "Internet time finished" +
                    "</h1>" +
                    $"<p style='color:red;'>Blocked by proxy server because internet time is finished ({cust.Group.TimeUsageStart:HH:mm}-{cust.Group.TimeUsageEnd:HH:mm}).</p>" +
                    "</body>" +
                    "</html>");
                return;
            }
        }

        private async Task ProxyServer_BeforeResponse(object sender, SessionEventArgs e)
        {
            SessionListItem item = null;
            await Task.Run(() =>
            {
                if (sessionDictionary.TryGetValue(e.HttpClient, out item))
                {
                    item.Update(e);
                }
            });

            //e.HttpClient.Response.Headers.AddHeader("X-Titanium-Header", "HTTP/2 works");

            //e.SetResponseBody(Encoding.ASCII.GetBytes("TITANIUMMMM!!!!"));

            if (item != null)
            {
                if (e.HttpClient.Response.HasBody)
                {
                    e.HttpClient.Response.KeepBody = true;
                    await e.GetResponseBody();

                    await Task.Run(() => { item.Update(e); });
                    if (item == SelectedSession)
                    {
                        await Task.Run(selectedSessionChanged);
                    }
                }
            }
        }

        private async Task ProxyServer_AfterResponse(object sender, SessionEventArgs e)
        {
            await Task.Run(() =>
            {
                if (sessionDictionary.TryGetValue(e.HttpClient, out var item))
                {
                    item.Exception = e.Exception;
                }
            });
        }

        private async Task<SessionListItem> addSession(SessionEventArgsBase e)
        {
            var item = createSessionListItem(e);
            Sessions.Insert(0, item);
            sessionDictionary.Add(e.HttpClient, item);
            foreach (var ev in SessionsChanged)
                await Task.Run(ev.InvokeAsync);

            return item;
        }

        private SessionListItem createSessionListItem(SessionEventArgsBase e)
        {
            lastSessionNumber++;
            bool isTunnelConnect = e is TunnelConnectSessionEventArgs;
            var item = new SessionListItem
            {
                Number = lastSessionNumber,
                ClientConnectionId = e.ClientConnectionId,
                ServerConnectionId = e.ServerConnectionId,
                HttpClient = e.HttpClient,
                ClientRemoteEndPoint = e.ClientRemoteEndPoint,
                ClientLocalEndPoint = e.ClientLocalEndPoint,
                IsTunnelConnect = isTunnelConnect
            };

            //if (isTunnelConnect || e.HttpClient.Request.UpgradeToWebSocket)
            e.DataReceived += async (sender, args) =>
            {
                var session = (SessionEventArgsBase)sender;
                if (sessionDictionary.TryGetValue(session.HttpClient, out var li))
                {
                    var connectRequest = session.HttpClient.ConnectRequest;
                    var tunnelType = connectRequest?.TunnelType ?? TunnelType.Unknown;
                    if (tunnelType != TunnelType.Unknown)
                    {
                        li.Protocol = TunnelTypeToString(tunnelType);
                    }

                    li.ReceivedDataCount += args.Count;
                    Customers.Add(e.ClientRemoteEndPoint.Address, args.Count, 0);

                    //if (tunnelType == TunnelType.Http2)
                    AppendTransferLog(session.GetHashCode() + (isTunnelConnect ? "_tunnel" : "") + "_received",
                        args.Buffer, args.Offset, args.Count);
                }
            };

            e.DataSent += async (sender, args) =>
            {
                var session = (SessionEventArgsBase)sender;
                if (sessionDictionary.TryGetValue(session.HttpClient, out var li))
                {
                    var connectRequest = session.HttpClient.ConnectRequest;
                    var tunnelType = connectRequest?.TunnelType ?? TunnelType.Unknown;
                    if (tunnelType != TunnelType.Unknown)
                    {
                        li.Protocol = TunnelTypeToString(tunnelType);
                    }

                    li.SentDataCount += args.Count;
                    Customers.Add(e.ClientRemoteEndPoint.Address, 0, args.Count);

                    //if (tunnelType == TunnelType.Http2)
                    AppendTransferLog(session.GetHashCode() + (isTunnelConnect ? "_tunnel" : "") + "_sent",
                        args.Buffer, args.Offset, args.Count);
                }
            };

            if (e is TunnelConnectSessionEventArgs te)
            {
                te.DecryptedDataReceived += (sender, args) =>
                {
                    var session = (SessionEventArgsBase)sender;
                    //var tunnelType = session.HttpClient.ConnectRequest?.TunnelType ?? TunnelType.Unknown;
                    //if (tunnelType == TunnelType.Http2)
                    AppendTransferLog(session.GetHashCode() + "_decrypted_received", args.Buffer, args.Offset,
                        args.Count);
                };

                te.DecryptedDataSent += (sender, args) =>
                {
                    var session = (SessionEventArgsBase)sender;
                    //var tunnelType = session.HttpClient.ConnectRequest?.TunnelType ?? TunnelType.Unknown;
                    //if (tunnelType == TunnelType.Http2)
                    AppendTransferLog(session.GetHashCode() + "_decrypted_sent", args.Buffer, args.Offset, args.Count);
                };
            }

            item.Update(e);
            return item;
        }

        private void AppendTransferLog(string fileName, byte[] buffer, int offset, int count)
        {
            //string basePath = @"c:\!titanium\";
            //using (var fs = new FileStream(basePath + fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            //{
            //    fs.Write(buffer, offset, count);
            //}
        }

        private string TunnelTypeToString(TunnelType tunnelType)
        {
            switch (tunnelType)
            {
                case TunnelType.Https:
                    return "https";
                case TunnelType.Websocket:
                    return "websocket";
                case TunnelType.Http2:
                    return "http2";
            }

            return null;
        }

        public void DeleteSession(SessionListItem selectedItem)
        {
            bool isSelected = false;
            Sessions.SuppressNotification = true;
            if (selectedItem == SelectedSession)
                isSelected = true;

            Sessions.Remove(selectedItem);
            sessionDictionary.Remove(selectedItem.HttpClient);

            Sessions.SuppressNotification = false;

            if (isSelected)
                SelectedSession = null;
        }

        public void SelectSession(SessionListItem selectedItem)
        {
            SelectedSession = selectedItem;
        }

        public void selectedSessionChanged()
        {
            if (SelectedSession == null) return;

            const int truncateLimit = 1024;

            var session = SelectedSession.HttpClient;
            var request = session.Request;
            var fullData = (request.IsBodyRead ? request.Body : null) ?? Array.Empty<byte>();
            var data = fullData;
            bool truncated = data.Length > truncateLimit;
            if (truncated)
                data = data.Take(truncateLimit).ToArray();

            //string hexStr = string.Join(" ", data.Select(x => x.ToString("X2")));
            var sb = new StringBuilder();
            sb.AppendLine("URI: " + request.RequestUri);
            sb.Append(request.HeaderText);
            sb.Append(request.Encoding.GetString(data));
            if (truncated)
            {
                sb.AppendLine();
                sb.Append($"Data is truncated after {truncateLimit} bytes");
            }

            sb.Append((request as ConnectRequest)?.ClientHelloInfo);
            selectedSession.Request = sb.ToString();

            var response = session.Response;
            fullData = (response.IsBodyRead ? response.Body : null) ?? Array.Empty<byte>();
            data = fullData;
            truncated = data.Length > truncateLimit;
            if (truncated)
            {
                data = data.Take(truncateLimit).ToArray();
            }

            //hexStr = string.Join(" ", data.Select(x => x.ToString("X2")));
            sb = new StringBuilder();
            sb.Append(response.HeaderText);
            sb.Append(response.Encoding.GetString(data));
            if (truncated)
            {
                sb.AppendLine();
                sb.Append($"Data is truncated after {truncateLimit} bytes");
            }

            sb.Append((response as ConnectResponse)?.ServerHelloInfo);
            if (SelectedSession.Exception != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(SelectedSession.Exception);
            }

            selectedSession.Response = sb.ToString();

            try
            {
                if (fullData.Length > 0)
                    using (var stream = new MemoryStream(fullData))
                        selectedSession.Img = stream;
            }
            catch
            {
                selectedSession.Img = null;
            }
        }

        bool proxyServerOn;
        private void ProxyOnOff()
        {
            if (proxyServerOn)
                proxyServer.SetAsSystemProxy((ExplicitProxyEndPoint)proxyServer.ProxyEndPoints[0], ProxyProtocolType.AllHttp);
            else
                proxyServer.RestoreOriginalProxySettings();

            proxyServerOn = !proxyServerOn;
        }
    }
}

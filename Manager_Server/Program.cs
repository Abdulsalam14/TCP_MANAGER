using Manager_Server;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var ip = IPAddress.Loopback;
var port = 27001;

var listener = new TcpListener(ip, port);
listener.Start();



while (true)
{
    var client = listener.AcceptTcpClient();
    var stream = client.GetStream();
    var br = new BinaryReader(stream);
    var bw = new BinaryWriter(stream);
    while (true)
    {
        var input = br.ReadString();
        var command = JsonSerializer.Deserialize<Command>(input);

        if (command is null) continue;
        Console.WriteLine(command.Text);
        Console.WriteLine(command.Param);
        switch (command.Text)
        {
            case Command.ProcessList:
                var processes = Process.GetProcesses();
                var processesNames = JsonSerializer
                    .Serialize(processes.Select(p => p.ProcessName));
                bw.Write(processesNames);
                break;
            case Command.Kill:
                var process=Process.GetProcesses().ToList().FirstOrDefault(p=>p.ProcessName==command.Param!.ToLower());
                if (process != null)
                {
                    process.Kill();
                    bw.Write($"{process.ProcessName} Killed");
                }
                else bw.Write("Your command is not correct");
                break;
            case Command.Run:
                try
                {
                    Process.Start($"{command.Param!.ToLower()}");
                    if (string.IsNullOrEmpty(command.Text)) Console.WriteLine("NUL INPUT");
                    bw.Write($"{command.Param} Runned");
                }
                catch (Exception)
                {
                    bw.Write("Your command is not correct");
                }
                break;
            default:
                break;
        }
    }
}
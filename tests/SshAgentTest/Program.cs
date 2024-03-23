using KeyWarden;
using KeyWarden.AgentProtocol;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

//var agent = new SshAgent(new TestAgent());
//await Task.Delay(1000000000);

var pipe = new NamedPipeClientStream(".", "openssh-ssh-agent", PipeDirection.InOut, PipeOptions.WriteThrough | PipeOptions.Asynchronous, System.Security.Principal.TokenImpersonationLevel.Identification, System.IO.HandleInheritability.Inheritable);

await pipe.ConnectAsync();

await using var client = new AgentClient()
{
	Stream = pipe,
};

var request = new BufferWriter();
request.WriteBlob(Convert.FromBase64String("AAAAC3NzaC1lZDI1NTE5AAAAIGwxX7gxgXMnT06x4g7wpdhsh3szsgemOFh4zDhBcfww"));
request.WriteString("hello");
request.WriteUInt32(4);
await client.SendMessage(new()
{
	Type = AgentMessageType.SignRequest,
	Contents = request.DataWritten,
});

var responseMsg = await client.ReadMessage();
var response = new BufferReader(responseMsg.Contents.ContiguousMemory);
var sig = response.ReadBlob();


int x = 0;

file class TestAgent : ISshAgentHandler
{
	private readonly List<SshKey> Keys = new()
	{
		new()
		{
			Name = "Test ECC key",
			PublicKey = Convert.FromBase64String("AAAAC3NzaC1lZDI1NTE5AAAAIHlaPQcEaq3nA+zPRW2aoP0d5eHx6nrnUJnLNMPuIlsX"),
			PrivateKey =
					"""
					-----BEGIN OPENSSH PRIVATE KEY-----
					b3BlbnNzaC1rZXktdjEAAAAABG5vbmUAAAAEbm9uZQAAAAAAAAABAAAAMwAAAAtzc2gtZW
					QyNTUxOQAAACB5Wj0HBGqt5wPsz0VtmqD9HeXh8ep651CZyzTD7iJbFwAAAJDthtsL7Ybb
					CwAAAAtzc2gtZWQyNTUxOQAAACB5Wj0HBGqt5wPsz0VtmqD9HeXh8ep651CZyzTD7iJbFw
					AAAEA6OG/DKrT98jac6G6cPagI6PH7jpTc5P+Bs/V26BDVKXlaPQcEaq3nA+zPRW2aoP0d
					5eHx6nrnUJnLNMPuIlsXAAAAC2theWxhQExvZ29zAQI=
					-----END OPENSSH PRIVATE KEY-----
					"""u8.ToArray(),
		},
		new()
		{
			Name = "Test RSA key",
			PublicKey = Convert.FromBase64String("AAAAB3NzaC1yc2EAAAADAQABAAABgQC4UEx1RJdbWgBzgoubzNZgE0EmkfM+4Gjva2yzUHQSusfezJLxTOWuGgC1AKtFGZCpTV2sOo5CLV/nu7BkCI4etgpzvEFo7MLVn55GTIxC8StRdmZvoWPkMgOoll58nNF8qRELp1SZ3gReOKZgXJBxUfAwaoKXhud4CMqkU9NTNO6HkRS9btd5Jzvbi0URswGS3eFKJ1IwnL8RvdVdS7rgs304bMaYqJN4eiO/IR7QJXFijkJNKEDUwWdevXpoBBMR19s8GzyJ+a961maLJ6jlGNxYcFZMSDmTZrH32JySQ+tnlKbwZejqyfhN3yHT0RO/Gu3rLn3ZFBs1de20LtxBiBLFDC2VusvPLS0OAXG//j5U/2z36w/RvA4QL85sJaAVNN8AjK+wGBXJ3AdrIKTgjLgYm0bSaixb6MxdoMKOBh+fFGbN5KG905h9G4lop++UYmLoQ/Chr3KZy2JPOqIkDDfpXAv/hHapds2zbZLcZUCaEDobvuyWxhMwoWBYheM="),
			PrivateKey =
				"""
				-----BEGIN OPENSSH PRIVATE KEY-----
				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
				-----END OPENSSH PRIVATE KEY-----
				"""u8.ToArray(),
		},
	};

	public ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct)
	{
		var procs = string.Join(" via ", info.Processes.Select(p => p.ProcessName));
		Console.WriteLine($"{info.Username} is requesting keys via {procs}");

		return new(Keys.Select(k => k with { PrivateKey = default }).ToArray());
	}

	public ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
	{
		var procs = string.Join(" via ", info.Processes.Select(p => p.ProcessName));

		var matchingKey = Keys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		Console.WriteLine($"{info.Username} is requesting access to key {(matchingKey.IsEmpty ? "unknown" : matchingKey.Name)} via {procs}");

		return new(matchingKey);
	}
}

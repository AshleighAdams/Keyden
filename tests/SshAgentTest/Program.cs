using KeyWarden;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

var agent = new SshAgent(new AgentK());
await Task.Delay(1000000000);

file class AgentK : ISshAgentHandler
{
	private readonly List<SshKey> Keys = new()
	{
		new()
		{
			Name = "Test key",
			PublicKey = Convert.FromBase64String("AAAAC3NzaC1lZDI1NTE5AAAAIHAfwGgfNUkZxwvrcLkoAAobq8Sv0Y+8SBZDzLcmoFJh"),
		},
		new()
		{
			Name = "Other more different key",
			PublicKey = Convert.FromBase64String("AAAAB3NzaC1yc2EAAAADAQABAAABAQC2c06vPjWmhQLJdHObnvhb3T4IoREfgdxrxf5kBVN4FRUSM1KUzoxGxP8Y5Gkm60nWsbVjKg+gYl3porTTgMFDQqv5zFo1Hgq7EuoGqmnASLy/hLH1EdQoSx1qiPYINMzZIcpRc/EGyZmrwhTYUUPG2fmj4CfnAiaiRlPP8/6LLPh50cb0TXFWV9dV/SHxi2iFqklQZ2IrIQyXm22SofTQoRpWgUVLVXykJVFuZ6TUsWIr83ZOkqAHENvejjNdViAAYE5b0FTyeNOEB7Hv7k3RXTFKXYy6yu5adxCixW2jaPVa2Okewxor26ezo6B+y9pXNwA3DokukizQemts2CmJ"),
		},
	};

	public ValueTask<IReadOnlyList<SshKey>> GetPublicKeys(ClientInfo info, CancellationToken ct)
	{
		var procs = string.Join(" via ", info.Processes.Select(p => p.ProcessName));
		Console.WriteLine($"{info.Username} is requesting keys {procs}");

		return new(Keys.Select(k => k with { PrivateKey = default }).ToArray());
	}

	public ValueTask<SshKey> GetPrivateKey(SshKey publicKey, ClientInfo info, CancellationToken ct)
	{
		var procs = string.Join(" via ", info.Processes.Select(p => p.ProcessName));
		Console.WriteLine($"{info.Username} via {procs} is requesting access to key {publicKey.Name}");

		var matchingKey = Keys
			.Where(k => k.PublicKey.Span.SequenceEqual(publicKey.PublicKey.Span))
			.FirstOrDefault();

		return new(matchingKey);
	}
}

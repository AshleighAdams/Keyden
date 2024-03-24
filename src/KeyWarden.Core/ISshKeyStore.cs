using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KeyWarden;

public interface ISshKeyStore
{
	Task SyncKeys(CancellationToken ct = default);
	IReadOnlyList<SshKey> PublicKeys { get; }
	ValueTask<SshKey> GetPrivateKey(SshKey publicKey, CancellationToken ct);
}

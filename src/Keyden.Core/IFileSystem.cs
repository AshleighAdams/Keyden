using System;
using System.IO;
using System.Threading.Tasks;

namespace Keyden;

public interface IFileSystem
{
	bool TryReadBytes(string path, out Memory<byte> contents);
	bool TryWriteBytes(string path, Memory<byte> contents);

	Task<(bool success, Memory<byte> contents)> TryReadBytesAsync(string path);
	Task<bool> TryWriteBytesAsync(string path, Memory<byte> contents);
}

public sealed class NullFileSystem : IFileSystem
{
	bool IFileSystem.TryReadBytes(string path, out Memory<byte> contents)
	{
		contents = Array.Empty<byte>();
		return false;
	}

	bool IFileSystem.TryWriteBytes(string path, Memory<byte> contents)
	{
		return false;
	}

	Task<(bool success, Memory<byte> contents)> IFileSystem.TryReadBytesAsync(string path)
	{
		return Task.FromResult<(bool success, Memory<byte> contents)>((false, Array.Empty<byte>()));
	}

	Task<bool> IFileSystem.TryWriteBytesAsync(string path, Memory<byte> contents)
	{
		return Task.FromResult(false);
	}

}

public sealed class SystemFileSystem : IFileSystem
{
	public string BasePath { get; }

	public SystemFileSystem(string basePath)
	{
		BasePath = basePath;
	}

	bool IFileSystem.TryReadBytes(string path, out Memory<byte> contents)
	{
		try
		{
			contents = File.ReadAllBytes(Path.Join(BasePath, path));
			return true;
		}
		catch
		{
			contents = Array.Empty<byte>();
			return false;
		}
	}

	bool IFileSystem.TryWriteBytes(string path, Memory<byte> contents)
	{
		try
		{
			File.WriteAllBytes(Path.Join(BasePath, path), contents.ToArray());
			return true;
		}
		catch
		{
			return false;
		}
	}

	async Task<(bool success, Memory<byte> contents)> IFileSystem.TryReadBytesAsync(string path)
	{
		try
		{
			var contents = await File.ReadAllBytesAsync(Path.Join(BasePath, path));
			return (true, contents);
		}
		catch
		{
			return (false, Array.Empty<byte>());
		}
	}

	async Task<bool> IFileSystem.TryWriteBytesAsync(string path, Memory<byte> contents)
	{
		try
		{
			await File.WriteAllBytesAsync(Path.Join(BasePath, path), contents.ToArray());
			return true;
		}
		catch
		{
			return false;
		}
	}
}

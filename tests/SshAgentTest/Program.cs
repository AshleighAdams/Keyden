using KeyWarden.SshAgent;

using System;
using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipes;
using System.Text;

if (false)
{
	var pipeClient = new NamedPipeClientStream(
		serverName: ".",
		pipeName: "openssh-ssh-agent",
		direction: PipeDirection.InOut,
		options: PipeOptions.Asynchronous);

	await pipeClient.ConnectAsync();

	var connection = new SshAgentTransport()
	{
		Stream = pipeClient,
	};

	await connection.SendMessage(new AgentMessage()
	{
		Type = AgentMessageType.RequestIdentities,
		Contents = new(Array.Empty<byte>()),
	});

	var serverResponse = await connection.ReadMessage();
	var bufferReader = new BufferReader(serverResponse.Contents.ContiguousMemory);

	int count = (int)bufferReader.ReadUInt32();
	for (int i = 0; i < count; i++)
	{
		if (i > 0)
			Console.WriteLine();

		var key = bufferReader.ReadBlob();
		var name = bufferReader.ReadString();

		var base64 = Convert.ToBase64String(key.Span);

		Console.WriteLine(name);
	}

	return;
}

var pipeServer = new NamedPipeServerStream(
	pipeName: "openssh-ssh-agent",
	direction: PipeDirection.InOut,
	maxNumberOfServerInstances: 1,
	transmissionMode: PipeTransmissionMode.Byte,
	options: PipeOptions.Asynchronous,
	inBufferSize: 5 *1024,
	outBufferSize: 5 *1024);

await pipeServer.WaitForConnectionAsync();

var test = new SshAgentTransport()
{
	Stream = pipeServer,
};

var rejectMessage = [DoesNotReturn] async () =>
{
	await test.SendMessage(new AgentMessage()
	{
		Type = AgentMessageType.Failure,
		Contents = new(),
	});
	Environment.Exit(1);
};

var msg = await test.ReadMessage();

if (msg.Type != AgentMessageType.RequestIdentities)
	await rejectMessage();

var response = new BufferWriter();

response.WriteUInt32(2);
response.WriteBlob(Convert.FromBase64String("AAAAC3NzaC1lZDI1NTE5AAAAIHAfwGgfNUkZxwvrcLkoAAobq8Sv0Y+8SBZDzLcmoFJh"));
response.WriteString("Test key");
response.WriteBlob(Convert.FromBase64String("AAAAB3NzaC1yc2EAAAADAQABAAABAQC2c06vPjWmhQLJdHObnvhb3T4IoREfgdxrxf5kBVN4FRUSM1KUzoxGxP8Y5Gkm60nWsbVjKg+gYl3porTTgMFDQqv5zFo1Hgq7EuoGqmnASLy/hLH1EdQoSx1qiPYINMzZIcpRc/EGyZmrwhTYUUPG2fmj4CfnAiaiRlPP8/6LLPh50cb0TXFWV9dV/SHxi2iFqklQZ2IrIQyXm22SofTQoRpWgUVLVXykJVFuZ6TUsWIr83ZOkqAHENvejjNdViAAYE5b0FTyeNOEB7Hv7k3RXTFKXYy6yu5adxCixW2jaPVa2Okewxor26ezo6B+y9pXNwA3DokukizQemts2CmJ"));
response.WriteString("Other more different key");

await test.SendMessage(new AgentMessage()
{
	Type = AgentMessageType.IdentitiesAnswer,
	Contents = response.DataWritten,
});

pipeServer.Close();

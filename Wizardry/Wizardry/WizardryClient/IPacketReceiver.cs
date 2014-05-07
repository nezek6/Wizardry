using Lidgren.Network;

namespace WizardryClient
{
	public interface IPacketReceiver
	{
		void ReceivePacket( NetIncomingMessage msg );
	}
}

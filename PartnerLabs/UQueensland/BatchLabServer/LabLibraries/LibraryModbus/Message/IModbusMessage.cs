namespace Modbus.Message
{
	/// <summary>
	/// A message built by the master (client) that initiates a Modbus transaction.
	/// </summary>
	public interface IModbusMessage
	{
		/// <summary>
		/// The function code tells the server what kind of action to perform.
		/// </summary>
		byte FunctionCode { get; set; }

		/// <summary>
		/// Address of the slave (server).
		/// </summary>
		byte SlaveAddress { get; set; }
		
		///<summary>
		/// Composition of the slave address and protocol data unit.
		///</summary>
		byte[] MessageFrame { get; }

		/// <summary>
		/// Composition of the function code and message data.
		/// </summary>
		byte[] ProtocolDataUnit { get; }

		/// <summary>
		/// A unique identifier assigned to a message when using the IP protocol.
		/// </summary>
		ushort TransactionID { get; set; }

		/// <summary>
		/// Initializes a modbus message from the specified message frame.
		/// </summary>
		/// <param name="frame">The frame.</param>
		void Initialize(byte[] frame);
	}
}

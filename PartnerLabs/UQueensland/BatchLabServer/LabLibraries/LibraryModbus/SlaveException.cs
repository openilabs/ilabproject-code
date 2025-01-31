using System;
using Modbus.Message;
using System.Runtime.Serialization;

namespace Modbus
{
	/// <summary>
	/// Represents slave errors that occur during communication.
	/// </summary>
	[Serializable]
	public class SlaveException : Exception
	{
		private readonly SlaveExceptionResponse _slaveExceptionResponse;

		private const string slaveAddressPropertyName = "SlaveAdress";
		private const string functionCodePropertyName = "FunctionCode";
		private const string slaveExceptionCodePropertyName = "SlaveExceptionCode";

		/// <summary>
		/// Initializes a new instance of the <see cref="SlaveException"/> class.
		/// </summary>
		public SlaveException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SlaveException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public SlaveException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SlaveException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public SlaveException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SlaveException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0). </exception>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is null. </exception>
		protected SlaveException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info != null)
			{
				_slaveExceptionResponse = new SlaveExceptionResponse(info.GetByte(slaveAddressPropertyName), info.GetByte(functionCodePropertyName), info.GetByte(slaveExceptionCodePropertyName));
			}
		}

		internal SlaveException(SlaveExceptionResponse slaveExceptionResponse)
		{
			_slaveExceptionResponse = slaveExceptionResponse;
		}

		internal SlaveException(string message, SlaveExceptionResponse slaveExceptionResponse)
			: base(message)
		{
			_slaveExceptionResponse = slaveExceptionResponse;
		}

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <value></value>
		/// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
		public override string Message
		{
			get
			{
				return String.Concat(base.Message, _slaveExceptionResponse != null ? String.Concat(Environment.NewLine, _slaveExceptionResponse) : String.Empty);
			}
		}

		/// <summary>
		/// Gets the response function code that caused the exception to occur, or 0.
		/// </summary>
		/// <value>The function code.</value>
		public byte FunctionCode
		{
			get
			{
				return _slaveExceptionResponse != null ? _slaveExceptionResponse.FunctionCode : (byte) 0;
			}
		}

		/// <summary>
		/// Gets the slave exception code, or 0.
		/// </summary>
		/// <value>The slave exception code.</value>
		public byte SlaveExceptionCode
		{
			get
			{
				return _slaveExceptionResponse != null ? _slaveExceptionResponse.SlaveExceptionCode : (byte) 0;
			}
		}

		/// <summary>
		/// Gets the slave address, or 0.
		/// </summary>
		/// <value>The slave address.</value>
		public byte SlaveAddress
		{
			get
			{
				return _slaveExceptionResponse != null ? _slaveExceptionResponse.SlaveAddress : (byte) 0;
			}
		}

		/// <summary>
		/// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The info parameter is a null reference (Nothing in Visual Basic). </exception>
		/// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/></PermissionSet>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			if (info != null && _slaveExceptionResponse != null)
			{
				info.AddValue(slaveAddressPropertyName, _slaveExceptionResponse.SlaveAddress);
				info.AddValue(functionCodePropertyName, _slaveExceptionResponse.FunctionCode);
				info.AddValue(slaveExceptionCodePropertyName, _slaveExceptionResponse.SlaveExceptionCode);
			}
		}
	}
}

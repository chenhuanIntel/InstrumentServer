using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Utility
{
	/// <summary>
	/// 
	/// </summary>
	public class CEventMessage
	{
		/// <summary>
		/// 
		/// </summary>
		protected DateTime _TimeStamp;
		/// <summary>
		/// 
		/// </summary>
		public DateTime TimeStamp
		{
			get
			{
				return _TimeStamp;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected string _strMessage;
		/// <summary>
		/// 
		/// </summary>
		public string strMessage
		{
			get
			{
				return _strMessage;
			}
		}

		//protected LogLevel _Level;
		//public LogLevel Level
		//{
		//	get
		//	{
		//		return _Level;
		//	}
		//}

		/// <summary>
		/// 
		/// </summary>
		protected int _nThreadID;
		/// <summary>
		/// 
		/// </summary>
		public int nThreadID
		{
			get
			{
				return _nThreadID;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected string _strLocationSource;
		/// <summary>
		/// 
		/// </summary>
		public string strLocationSource
		{
			get
			{
				return _strLocationSource;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		protected int _nQueue;
		/// <summary>
		/// 
		/// </summary>
		public int nQueue
		{
			get
			{
				return _nQueue;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="myMessage"></param>
		/// <param name="myQueue"></param>
		/// <param name="myLocationSource"></param>
		/// <param name="myThreadID"></param>
		public CEventMessage(string myMessage, int myQueue = -1, string myLocationSource = "", int myThreadID = 0)
		{
			_TimeStamp = DateTime.Now;
			_strMessage = myMessage;
			//_Level = myLevel;
			_nThreadID = myThreadID;
			_strLocationSource = myLocationSource;
			_nQueue = myQueue;
		}
	}
}

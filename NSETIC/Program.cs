using System;
using NSETIC;
namespace NSETIC
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("NSETIC - Novaschema SATS export to iCalendar converter");
			Console.WriteLine ("Test version 1");
			NSETIC test = new NSETIC ();
			test.Load ("DATA.TXT");
			test.GenerateICS ();
		}
	}
}

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NSETIC
{
	public class Subject
	{
		public string ID {get;set;}
		public string GUID {get;set;}
		public string Category {get;set;} 
		public string FullText {get;set;} 
		public string BackgroundColor {get;set;} 
		public string TextColor {get;set;} 
		public string EvaluationNumber {get;set;} 

		public Subject(string[] args){
			if(args.GetLength(0) == 7){
				ID = args [0];
				GUID = args [1];
				Category = args [2];
				FullText = args [3];
				BackgroundColor = args [4];
				TextColor = args [5];
				EvaluationNumber = args [6];
			}
		}
	}

	public class Teacher
	{
		public string ID {get;set;}
		public string GUID {get;set;}
		public string Category {get;set;} 
		public string PersonalCode {get;set;} 
		public string LastName {get;set;} 
		public string Title {get;set;} 
		public string FirstName {get;set;} 
		public string Phone {get;set;} 
		public string Email {get;set;} 
		public string EvaluationNumber {get;set;}

		public Teacher(string[] args){
			if (args.GetLength (0) == 10) {
				ID = args [0];
				GUID = args [1];
				Category = args [2];
				PersonalCode = args [3];
				LastName = args [4];
				Title = args [5];
				FirstName = args [6];
				Phone = args [7];
				Email = args [8];
				EvaluationNumber = args [9];
			} else {
				ID = "BADTEACHER";
			}
		}
	}

	public class Lesson
	{
		public string ID {get;set;}
		public string GUID {get;set;}
		public string DayOfWeek {get;set;} 
		public string Starttime {get;set;} 
		public string Length {get;set;} 
		public string Course {get;set;} 
		public string Subject {get;set;} 
		public string Teacher {get;set;} 
		public string Group {get;set;} 
		public string Room {get;set;}
		public string Period {get;set;}
		public string Week {get;set;}
		public string ActualWeeks {get;set;}

		public Lesson(string[] args){
			if(args.GetLength(0) == 13){
				ID = args [0];
				GUID = args [1];
				DayOfWeek = args [2];
				Starttime = args [3];
				Length = args [4];
				Course = args [5];
				Subject = args [6];
				Teacher = args [7];
				Group = args [8];
				Room = args [9];
				Period = args [10];
				Week = args [11];
				ActualWeeks = args [12];
			}
		}
	}

	class NSETIC
	{
		private List<Subject> 	subjects;
		private List<Teacher> 	teachers;
		private List<Lesson> 	lessons;
		private int				baseyear, offsetyear, offsetmonth;

		public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
		{
			DateTime jan1 = new DateTime(year, 1, 1,0,0,0);
			int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

			DateTime firstThursday = jan1.AddDays(daysOffset);
			var cal = CultureInfo.CurrentCulture.Calendar;
			int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

			var weekNum = weekOfYear;
			if (firstWeek <= 1)
			{
				weekNum -= 1;
			}
			var result = firstThursday.AddDays(weekNum * 7);
			return result.AddDays(-3);
		}

		public void Load(string filename)
		{
			if (File.Exists(filename)){
				string[] lines = System.IO.File.ReadAllLines(filename,Encoding.GetEncoding(28591));

				int currentLine = 0;

				subjects = new List<Subject>();
				teachers = new List<Teacher>();
				lessons = new List<Lesson>();

				// todo why does it read a blank line into teachers?
				while (currentLine < lines.GetLength(0))
				{
					if (lines [currentLine].Split ('=') [0].Equals ("BaseYear")) {
						baseyear = Int32.Parse(lines [currentLine].Split ('=') [1]);
					}
					if (lines [currentLine].Split ('=') [0].Equals ("OffsetYear")) {
						offsetyear = Int32.Parse(lines [currentLine].Split ('=') [1]);
					}
					if (lines [currentLine].Split ('=') [0].Equals ("OffsetMonth")) {
						offsetmonth = Int32.Parse(lines [currentLine].Split ('=') [1]);
					}
					if (lines[currentLine].Equals (@"Subject (6400)", StringComparison.OrdinalIgnoreCase)) {
						int lineNr = currentLine+3;
						while (lineNr < lines.GetLength(0)) {
							if (lines [lineNr].Length == 0) 
								break;

							subjects.Add(new Subject (lines [lineNr].Split ('\t')));
							lineNr++;
						}
					}
					if (lines[currentLine].Equals (@"Teacher (6000)", StringComparison.OrdinalIgnoreCase)) {
						int lineNr = currentLine+3;
						while (lineNr < lines.GetLength(0)) {
							if (lines [lineNr].Length == 0)
								break;

							teachers.Add(new Teacher (lines [lineNr].Split ('\t')));
							lineNr++;
						}
					}
					if (lines[currentLine].Equals (@"Lesson (7100)", StringComparison.OrdinalIgnoreCase)) {
						int lineNr = currentLine+3;
						while (lineNr < lines.GetLength(0)) {
							if (lines [lineNr].Length == 0)
								break;

							lessons.Add(new Lesson (lines [lineNr].Split ('\t')));
							lineNr++;
						}
					}
					currentLine++;
				}
			}
			else {
				Console.WriteLine("File not loaded");
			}
		}

		public void GenerateICS()
		{
			if (baseyear != 0) {
				int year = baseyear + offsetyear;
				foreach (Teacher t in teachers) {

					if (!t.ID.Equals ("BADTEACHER")) {
						string output = "BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//villy/billy/NONSGML v1.0//EN\nBEGIN:VEVENT";
						List<Lesson> currentLessons = lessons.FindAll (x => x.Teacher == t.ID);

						foreach (Lesson i in currentLessons) {
							string[] weeks = i.ActualWeeks.Replace (" ", "").Split (',');
							foreach (string week in weeks) {
								int startweek;
								if (!Int32.TryParse (week.Split ('-') [0], out startweek)) {
									startweek = 0;
								}
								int endweek;
								if (week.Split ('-').GetLength (0) > 1) {
									if (!Int32.TryParse (week.Split ('-') [1], out endweek)) {
										endweek = startweek;
									}
								} else {
									endweek = startweek;
								}

								DateTime start = FirstDateOfWeekISO8601 (year, startweek);
								DateTime end = FirstDateOfWeekISO8601 (year, endweek);

								double starthour = Convert.ToDouble (Int32.Parse (i.Starttime.Split (':') [0]));
								double startminute = Convert.ToDouble (Int32.Parse (i.Starttime.Split (':') [1]));
								double minutestillend = Convert.ToDouble (Int32.Parse (i.Length));

								output += "BEGIN:VEVENT\n";
								output += "UID:" + Guid.NewGuid () + "@villy.net\n";
								output += "DTSTART:" + start.AddDays (Double.Parse (i.DayOfWeek)).ToString ("yyyyMMdd") + "T" + start.AddHours (starthour).AddMinutes (startminute).ToString ("HHmm") + "00Z\n";
								output += "DTEND:" + start.AddDays (Double.Parse (i.DayOfWeek)).ToString ("yyyyMMdd") + "T" + start.AddHours (starthour).AddMinutes (startminute).AddMinutes (minutestillend).ToString ("HHmm") + "00Z\n";
								output += "RRULE:FREQ=WEEKLY;UNTIL=" + end.AddDays (Double.Parse (i.DayOfWeek)).ToString ("yyyyMMdd") + "T" + end.AddHours (starthour).AddMinutes (startminute).AddMinutes (minutestillend).ToString ("HHmm") + "00Z\n";
								output += "SUMMARY:" + subjects.Find (x => x.ID == i.Subject).FullText + "\n";
								output += "END:VEVENT\n";
							}
						}

						output += "END:VCALENDAR\n";

						System.IO.File.WriteAllText (t.FirstName + " " + t.LastName + " (" + t.ID + ").ics", output);
					}
				}
			}
		}
	}
}
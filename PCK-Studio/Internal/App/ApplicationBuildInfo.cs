/* Copyright (c) 2023-present miku-666, MattNL
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.IO;
using System.Reflection;

namespace PckStudio.Internal.App
{
    static internal class ApplicationBuildInfo
    {
        // this is to specify which build release this is. This is manually updated for now
        // TODO: add different chars for different configurations
        private const string BuildType = "c";
        private static System.Globalization.Calendar _buildCalendar;
        private static DateTime date = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
        private static string _betaBuildVersion;

        public static string BetaBuildVersion
        {
            get
            {
                // adopted Minecraft Java Edition Snapshot format (YYwWWn)
                // to keep track of work in progress features and builds
                _buildCalendar ??= new System.Globalization.CultureInfo("en-US").Calendar;
                return _betaBuildVersion ??= string.Format("#{0}w{1}{2}",
                    date.ToString("yy"),
                    _buildCalendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday),
                    BuildType);
            }
        }
    }
}

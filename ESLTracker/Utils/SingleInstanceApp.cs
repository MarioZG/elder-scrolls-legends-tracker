using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESLTracker.Utils
{
    public class SingleInstanceApp :IDisposable
    {
        bool hasMutexHandle = false;
        Mutex mutex;

        public void Dispose()
        {
            // edited by acidzombie24, added if statement
            if (hasMutexHandle)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
        }

        /// <summary>
        /// bassed on http://stackoverflow.com/questions/229565/what-is-a-good-pattern-for-using-a-global-mutex-in-c/229567
        /// </summary>
        /// <returns></returns>
        public bool CheckInstance()
        {
            // get application GUID as defined in AssemblyInfo.cs
            string appGuid =
                ((GuidAttribute)Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(GuidAttribute), false).
                        GetValue(0)).Value.ToString();

            // unique id for global mutex - Global prefix means it is global to the machine
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);

            if(Debugger.IsAttached)
            {
                //lets allow run app and debug
                mutexId += "Debugging";
            }

            // Need a place to store a return value in Mutex() constructor call
            bool createdNew;

            // edited by Jeremy Wiebe to add example of setting up security for multi-user usage
            // edited by 'Marc' to work also on localized systems (don't use just "Everyone") 
            var allowEveryoneRule =
                new MutexAccessRule(
                    new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                    MutexRights.FullControl, AccessControlType.Allow);
            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);

            try
            {
                // note, you may want to time out here instead of waiting forever
                // edited by acidzombie24
                // mutex.WaitOne(Timeout.Infinite, false);
                hasMutexHandle = mutex.WaitOne(TimeSpan.Zero, false);
                //if (hasMutexHandle == false)
                //    throw new TimeoutException("Timeout waiting for exclusive access");
            }
            catch (AbandonedMutexException)
            {
                // Log the fact that the mutex was abandoned in another process,
                // it will still get acquired
                hasMutexHandle = true;
            }

            return hasMutexHandle;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManagedLzma.LZMA.Master
{
    partial class LZMA
    {
        #region CThread

        internal sealed class CThread
        {
            internal System.Threading.Thread _thread;
        }

        internal static void Thread_Construct(out CThread p)
        {
            p = null;
        }

        internal static bool Thread_WasCreated(CThread p)
        {
            return p != null;
        }

        internal static void Thread_Close(ref CThread p)
        {
            if(p != null)
            {
                if(SyncTrace.Enable)
                    Trace.MatchThreadClose(p._thread);
                else
                    p._thread.Join();
            }

            p = null;
        }

        internal static SRes Thread_Wait(CThread p)
        {
            p._thread.Join();
            if(SyncTrace.Enable)
                Trace.MatchThreadWait(p._thread);
            return TSZ("Thread_Wait");
        }

        internal static SRes Thread_Create(out CThread p, Action func)
        {
            p = new CThread();
            if(SyncTrace.Enable)
            {
                p._thread = Trace.MatchThreadStart(func);
            }
            else
            {
                p._thread = new System.Threading.Thread(delegate() { func(); });
                p._thread.Start();
            }
            return TSZ("Thread_Create");
        }

        #endregion

        #region CEvent

        // this is a win32 autoreset event

        internal sealed class CEvent
        {
            public System.Threading.AutoResetEvent Event;
        }

        internal static void Event_Construct(out CEvent p)
        {
            p = null;
        }

        internal static bool Event_IsCreated(CEvent p)
        {
            return p != null;
        }

        internal static void Event_Close(ref CEvent p)
        {
            if(p != null)
            {
                if(SyncTrace.Enable)
                    Trace.MatchObjectDestroy(p, "Event_Close");
                else
                    p.Event.Close();
            }
            p = null;
        }

        internal static SRes Event_Wait(CEvent p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "Event_Wait");
            else
                p.Event.WaitOne();
            return TSZ("Event_Wait");
        }

        internal static SRes Event_Set(CEvent p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "Event_Set");
            else
                p.Event.Set();
            return TSZ("Event_Set");
        }

        internal static SRes Event_Reset(CEvent p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "Event_Reset");
            else
                p.Event.Reset();
            return TSZ("Event_Reset");
        }

        internal static SRes AutoResetEvent_CreateNotSignaled(out CEvent p)
        {
            p = new CEvent();
            if(SyncTrace.Enable)
                Trace.MatchObjectCreate(p, "Event_Create");
            else
                p.Event = new System.Threading.AutoResetEvent(false);
            return TSZ("Event_Create");
        }

        #endregion

        #region CSemaphore

        internal sealed class CSemaphore
        {
            public System.Threading.Semaphore Semaphore;
        }

        internal static void Semaphore_Construct(out CSemaphore p)
        {
            p = null;
        }

        internal static void Semaphore_Close(ref CSemaphore p)
        {
            if(p != null)
            {
                if(SyncTrace.Enable)
                    Trace.MatchObjectDestroy(p, "Semaphore_Close");
                else
                    p.Semaphore.Close();
            }

            p = null;
        }

        internal static SRes Semaphore_Wait(CSemaphore p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "Semaphore_Wait");
            else
                p.Semaphore.WaitOne();
            return TSZ("Semaphore_Wait");
        }

        internal static SRes Semaphore_Create(out CSemaphore p, uint initCount, uint maxCount)
        {
            p = new CSemaphore();
            if(SyncTrace.Enable)
            {
                Trace.MatchObjectCreate(p, "Semaphore_Create");
                Trace.Match((int)initCount, (int)maxCount);
            }
            else
            {
                p.Semaphore = new System.Threading.Semaphore(checked((int)initCount), checked((int)maxCount));
            }
            return TSZ("Semaphore_Create");
        }

        internal static SRes Semaphore_Release1(CSemaphore p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "Semaphore_Release");
            else
                p.Semaphore.Release();
            return TSZ("Semaphore_Release");
        }

        #endregion

        #region CCriticalSection

        internal sealed class CCriticalSection { }

        internal static SRes CriticalSection_Init(out CCriticalSection p)
        {
            p = new CCriticalSection();
            if(SyncTrace.Enable)
                Trace.MatchObjectCreate(p, "CriticalSection_Init");
            return SZ_OK; // never fails in C code either
        }

        internal static void CriticalSection_Delete(CCriticalSection p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectDestroy(p, "CriticalSection_Delete");
        }

        internal static void CriticalSection_Enter(CCriticalSection p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "CriticalSection_Enter");
            else
                System.Threading.Monitor.Enter(p);
        }

        internal static void CriticalSection_Leave(CCriticalSection p)
        {
            if(SyncTrace.Enable)
                Trace.MatchObjectWait(p, "CriticalSection_Leave");
            else
                System.Threading.Monitor.Exit(p);
        }

        #endregion
    }
}

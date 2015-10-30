using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GeometryTutorLib.Pebbler
{
    //
    // This code is based on the MSDN example for Producer / Consumer in C#
    //
    public class SharedPebbledNodeList<A>
    {
        // This is shared data structure between the producer (Pebbler) and consumer (path generator) threads
        private List<PebblerHyperEdge<A>> edgeList;

        public SharedPebbledNodeList()
        {
            edgeList = new List<PebblerHyperEdge<A>>();
        }

        // State flag
        private bool writerFlag = false;  // on when writing
        private bool readerFlag = false;  // on when reading
        private bool writingComplete = false;

        public void SetWritingComplete()
        {
            writingComplete = true;
        }

        public bool IsReadingAndWritingComplete()
        {
            return writingComplete && !edgeList.Any();
        }

        //
        // Consumer method
        //
        public PebblerHyperEdge<A> ReadEdge()
        {
            // If writing is known to be done and the list is empty, this is an error
            if (IsReadingAndWritingComplete()) return null;

            PebblerHyperEdge<A> readEdge = null;

            // Enter synchronization block
            lock (this)
            {
                // Wait until WriteEdge produces OR is done producing a new edge
                if (writerFlag || !edgeList.Any())
                {
                    try
                    {
                        // Waits for the Monitor.Pulse in WriteEdge
                        // Add a timeout in case nothing is ever written...
                        Monitor.Wait(this /* , new TimeSpan(1000)*/);

                        // If this timed out, return invalid
                        //if (!edgeList.Any()) return null;
                    }
                    catch (SynchronizationLockException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }

                readerFlag = true;

                // Consume the edge
                readEdge = edgeList[0];
                edgeList.RemoveAt(0);

                // Reset the state flag to say producing is done.
                readerFlag = false;

                // Pulse tells WriteEdge that ReadEdge is done.
                Monitor.Pulse(this);
            }

            return readEdge;
        }

        //
        // Producer method
        //
        public void WriteEdge(PebblerHyperEdge<A> edgeToWrite)
        {
            // Enter synchronization block
            lock (this)
            {
                if (readerFlag)
                {   
                    // Wait until ReadEdge is done consuming.
                    try
                    {
                        // Wait for the Monitor.Pulse in Read
                        Monitor.Wait(this);
                    }
                    catch (SynchronizationLockException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }

                writerFlag = true;

                // Produce
                edgeList.Add(edgeToWrite);

                // Reset the state flag to say producing is done.
                writerFlag = false;

                // Pulse tells Read that Write is complete. 
                Monitor.Pulse(this);
            }
        }
    }
}

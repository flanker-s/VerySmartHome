﻿using SmartBulbColor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartBulbColor.Tools
{
    class AmbientLightStreamer
    {
        LinkedList<BulbColor> BulbsForStreaming;
        List<BulbColor> LostBulbs;

        readonly Thread AmbilightThread;
        readonly ManualResetEvent AmbilightTrigger;
        public ScreenColorAnalyzer ColorAnalyzer;

        public AmbientLightStreamer()
        {
            ColorAnalyzer = new ScreenColorAnalyzer();

            AmbilightThread = new Thread(new ThreadStart(StreamAmbientLightHSL));
            AmbilightThread.IsBackground = true;
            AmbilightTrigger = new ManualResetEvent(true);

            LostBulbs = new List<BulbColor>();
        }

        public void SetBulbsForStreaming(LinkedList<BulbColor> bulbs)
        {
            BulbsForStreaming = bulbs;
        }

        public void StartSreaming()
        {
            if (AmbilightThread.IsAlive)
            {
                AmbilightTrigger.Set();
            }
            else
            {
                AmbilightThread.Start();
            }
        }
        public void StopStreaming()
        {
            AmbilightTrigger.Reset();
            LostBulbs.Clear();
        }
        void StreamAmbientLightHSL()
        {
            HSBColor color;
            int previosHue = 0;
            while (true)
            {
                AmbilightTrigger.WaitOne(Timeout.Infinite);

                color = ColorAnalyzer.GetMostCommonColorHSL();

                var bright = color.Brightness;
                var hue = (bright < 1) ? previosHue : color.Hue;
                var sat = color.Saturation;
                foreach (var bulb in BulbsForStreaming)
                {
                    string command = $"{{\"id\":{bulb.Id},\"method\":\"set_scene\",\"params\":[\"hsv\", {hue}, {sat}, {bright}]}}\r\n";
                    byte[] commandBuffer = Encoding.UTF8.GetBytes(command);
                    try
                    {
                        bulb.AcceptedClient.Send(commandBuffer);
                    }
                    catch (Exception e)
                    {
                        bulb.AcceptedClient.Dispose();
                        bulb.AcceptedClient = null;
                        bulb.IsMusicModeOn = false;
                        LostBulbs.Add(bulb);
                        if ((BulbsForStreaming.Count - 1) == 0)
                            StopStreaming();
                    }
                }
                if (LostBulbs.Count != 0)
                {
                    foreach (var lostBulb in LostBulbs)
                    {
                        BulbsForStreaming.Remove(lostBulb);
                    }
                }
                previosHue = color.Hue;
                Thread.Sleep(60);
            }
        }
    }
}

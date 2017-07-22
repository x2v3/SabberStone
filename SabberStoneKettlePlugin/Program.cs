﻿using Kettle.Framework;
using System;
using System.Collections.ObjectModel;
using Kettle.Protocol;

namespace SabberStoneKettlePlugin
{
    class Program
    {
        internal const string IDENTIFIER = "Sabberstone Kettle Client";
        internal const string PROVIDER = "Stove Team";

        internal const int DESTRUCT_TIMEOUT = 5;

        static void Main(string[] args)
        {
            if (!KettleFramework.Init(args))
            {
                Console.Error.WriteLine("Init KettleFramework: {0}", KettleFramework.LastError);
                return;
            }

            if (!KettleFramework.Options.EnableEvents)
            {
                Console.Error.WriteLine("This module must be launched with enabled events in order to work!");
                goto CLEANUP;
            }

            KettleFramework.PreventTerminalInterrupt(null, DESTRUCT_TIMEOUT);

            SimulatorPurpose purpose = SimulatorPurpose.Simulator;
            int maxInstances = 20;
            Supported supportedDetails = new Supported()
            {
                Scenario = new ObservableCollection<GameScenarioEnum>()
                        {
                            GameScenarioEnum.Match_Standard,
                            GameScenarioEnum.Match_Wild
                        },
                GameID = new ObservableCollection<string>() { "default" }
            };

            try
            {
                if (KettleFramework.IsSlave())
                {
                    if (new SlaveCode(purpose, maxInstances, supportedDetails)?.Enter() != true)
                    {
                        Console.Error.WriteLine("SLAVE - Error occurred!");
                    }
                }
                else
                {
                    if (new MasterCode()?.Enter() != true)
                    {
                        Console.Error.WriteLine("MASTER - Error occurred!");
                    }
                }

            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unhandled exception!");
                Console.Error.WriteLine(e.Message);
            }

            CLEANUP:
            if (!String.IsNullOrWhiteSpace(KettleFramework.LastError))
            {
                Console.Error.WriteLine("KettleFramework's last error: {0}", KettleFramework.LastError);
            }

            if (!KettleFramework.Destruct(DESTRUCT_TIMEOUT))
            {
                Console.Error.WriteLine("Destruct KettleFramework: {0}", KettleFramework.LastError);
            }
        }
    }
}
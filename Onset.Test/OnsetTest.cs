﻿using System;
using System.Collections.Generic;
using System.Text;
using Onset.Plugin;

namespace Onset.Test
{

    [Meta("onset-test", 1, "1.0")]
    public class OnsetTest : OnsetPlugin
    {
        public override void Load()
        {
            Logger.Info("Hallo von Onset Test!");
        }

        public override void Unload()
        {
        }
    }
}
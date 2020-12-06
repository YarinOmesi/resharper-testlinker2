package com.jetbrains.rider.plugins.testlinker2

import com.jetbrains.rider.actions.base.RiderAnAction
import icons.ReSharperIcons.UnitTesting

class GotoLinkedTypes2Action : RiderAnAction(
        "GotoLinkedTypes2Action",
        "Goto Linked Types (Test/Production)",
        null,
        UnitTesting.TestFixtureToolWindow)

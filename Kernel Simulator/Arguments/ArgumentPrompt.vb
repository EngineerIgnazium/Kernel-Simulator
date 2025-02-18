﻿
'    Kernel Simulator  Copyright (C) 2018-2019  EoflaOE
'
'    This file is part of Kernel Simulator
'
'    Kernel Simulator is free software: you can redistribute it and/or modify
'    it under the terms of the GNU General Public License as published by
'    the Free Software Foundation, either version 3 of the License, or
'    (at your option) any later version.
'
'    Kernel Simulator is distributed in the hope that it will be useful,
'    but WITHOUT ANY WARRANTY; without even the implied warranty of
'    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'    GNU General Public License for more details.
'
'    You should have received a copy of the GNU General Public License
'    along with this program.  If not, see <https://www.gnu.org/licenses/>.

Module ArgumentPrompt

    'Variables
    Public answerargs As String                                                     'Input for arguments

    Sub PromptArgs(Optional ByVal InjMode As Boolean = False)

        'Checks if the arguments are injected
        If argsInjected = True Then
            argsInjected = False
            ParseArguments()
        Else
            'Shows available arguments and prompts for it
            W(DoTranslation("Available arguments: {0}", currentLang) + vbNewLine +
              DoTranslation("Arguments ('help' for help): ", currentLang), False, ColTypes.Input, String.Join(", ", AvailableArgs))
            answerargs = Console.ReadLine()

            'Make a kernel check for arguments later if anything is entered
            If answerargs <> Nothing And InjMode = False Then
                argsFlag = True
            ElseIf answerargs <> Nothing And InjMode = True Then
                argsInjected = True
                W(DoTranslation("Injected arguments will be scheduled to run at next reboot.", currentLang), True, ColTypes.Neutral)
            ElseIf answerargs = "q" And InjMode = True Then
                W(DoTranslation("Argument Injection has been cancelled.", currentLang), True, ColTypes.Neutral)
            End If
        End If

    End Sub

End Module

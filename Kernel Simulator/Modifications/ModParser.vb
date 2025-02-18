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

Imports System.CodeDom.Compiler
Imports System.Reflection
Imports Microsoft.CSharp

Public Module ModParser

    'Variables
    Public Interface IScript
        Sub StartMod()
        Sub StopMod()
        Property Cmd As String
        Property Def As String
        Property Name As String
        Property Version As String
        Sub PerformCmd(Optional ByVal args As String = "")
        Sub InitEvents(ByVal ev As String)
    End Interface
    Public scripts As New Dictionary(Of String, IScript)
    Private ReadOnly modPath As String = paths("Mods")

    '------------------------------------------- Generators -------------------------------------------
    Private Function GenMod(ByVal PLang As String, ByVal code As String) As IScript
        Dim provider As CodeDomProvider
        Wdbg($"Language detected: {PLang}")
        If PLang = "C#" Then
            provider = New CSharpCodeProvider
        ElseIf PLang = "VB.NET" Then
            provider = New VBCodeProvider
        Else
            Exit Function
        End If
        Dim prm As New CompilerParameters With {
                .GenerateExecutable = False,
                .GenerateInMemory = True
           }

        'Add referenced assemblies
        Wdbg("Referenced assemblies will be added.")
        prm.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly.Location) 'It should reference itself
        prm.ReferencedAssemblies.Add("System.dll")
        prm.ReferencedAssemblies.Add("System.Core.dll")
        prm.ReferencedAssemblies.Add("System.Data.dll")
        prm.ReferencedAssemblies.Add("System.DirectoryServices.dll")
        prm.ReferencedAssemblies.Add("System.Xml.dll")
        prm.ReferencedAssemblies.Add("System.Xml.Linq.dll")
        Wdbg("Referenced assemblies added.")

        'Try to compile
        Dim namespc As String = GetType(IScript).Namespace
        Dim modCode() As String
        If PLang = "VB.NET" Then
            modCode = {$"Imports {namespc}{vbNewLine}{code}"}
        ElseIf PLang = "C#" Then
            modCode = {$"using {namespc};{vbNewLine}{code}"}
        End If
#Disable Warning BC42104
        Dim res As CompilerResults = provider.CompileAssemblyFromSource(prm, modCode)
#Enable Warning BC42104

        'Check to see if there are compilation errors
        Wdbg("Has errors: {0}", res.Errors.HasErrors)
        Wdbg("Has warnings: {0}", res.Errors.HasWarnings)
        If res.Errors.HasErrors Then
            W(DoTranslation("Mod can't be loaded because of the following: ", currentLang), True, ColTypes.Neutral)
            For Each errorName In res.Errors
                W(errorName.ToString, True, ColTypes.Neutral)
                Wdbg(errorName.ToString)
            Next
            Exit Function
        End If
        For Each t As Type In res.CompiledAssembly.GetTypes()
            If t.GetInterface(GetType(IScript).Name) IsNot Nothing Then Return CType(res.CompiledAssembly.CreateInstance(t.Name), IScript)
        Next
    End Function

    '------------------------------------------- Parsers -------------------------------------------
    Sub ParseMods(ByVal StartStop As Boolean)
        'StartStop: If true, the mods start, otherwise, the mod stops.
        If Not FileIO.FileSystem.DirectoryExists(modPath) Then FileIO.FileSystem.CreateDirectory(modPath)
        Dim count As Integer = FileIO.FileSystem.GetFiles(modPath).Count
        If (count <> 0) And StartStop = True Then
            W(DoTranslation("mod: Loading mods...", currentLang), True, ColTypes.Neutral)
            Wdbg("Mods are being loaded. Total mods with screensavers = {0}", count)
        ElseIf (count <> 0) And StartStop = False Then
            W(DoTranslation("mod: Stopping mods...", currentLang), True, ColTypes.Neutral)
            Wdbg("Mods are being stopped. Total mods with screensavers = {0}", count)
        End If
        If StartStop = False Then
            For Each script As IScript In scripts.Values
                script.StopMod()
                Wdbg("script.StopMod() initialized. Mod name: {0} | Version: {0}", script.Name, script.Version)
                If script.Name <> "" And script.Version <> "" Then W("{0} v{1} stopped", True, ColTypes.Neutral, script.Name, script.Version)
            Next
        Else
            For Each modFile As String In FileIO.FileSystem.GetFiles(modPath)
                StartParse(modFile, StartStop)
            Next
        End If
    End Sub
    Sub StartParse(ByVal modFile As String, Optional ByVal StartStop As Boolean = True)
        modFile = modFile.Replace(modPath, "")
        If Not modFile.EndsWith(".m") Then
            'Ignore all mods who doesn't end with .m
            Wdbg("Unsupported file type for mod file {0}.", modFile)
        ElseIf modFile.EndsWith("SS.m") Then
            'Ignore all mods who ends with SS.m
            Wdbg("Mod file {0} is a screensaver and is ignored.", modFile)
        ElseIf modFile.EndsWith("CS.m") Then
            'Mod has a language of C#
            Dim script As IScript = GenMod("C#", IO.File.ReadAllText(modPath + modFile))
            FinalizeMods(script, modFile, StartStop)
        Else
            Dim script As IScript = GenMod("VB.NET", IO.File.ReadAllText(modPath + modFile))
            FinalizeMods(script, modFile, StartStop)
        End If
    End Sub

    '------------------------------------------- Finalizer -------------------------------------------
    Sub FinalizeMods(ByVal script As IScript, ByVal modFile As String, Optional ByVal StartStop As Boolean = True)
        If Not IsNothing(script) Then
            script.StartMod()
            Wdbg("script.StartMod() initialized. Mod name: {0} | Version: {0}", script.Name, script.Version)
            If script.Name = "" Then
                Wdbg("No name for {0}", modFile)
                W(DoTranslation("Mod {0} does not have the name. Review the source code.", currentLang), True, ColTypes.Neutral, modFile)
                scripts.Add(script.Cmd, script)
            Else
                Wdbg("There is a name for {0}", modFile)
                scripts.Add(script.Name, script)
            End If
            If script.Version = "" And script.Name <> "" Then
                Wdbg("{0}.Version = """" | {0}.Name = {1}", modFile, script.Name)
                W(DoTranslation("Mod {0} does not have the version.", currentLang), True, ColTypes.Neutral, script.Name)
            ElseIf script.Name <> "" And script.Version <> "" Then
                Wdbg("{0}.Version = {2} | {0}.Name = {1}", modFile, script.Name, script.Version)
                W(DoTranslation("{0} v{1} started", currentLang), True, ColTypes.Neutral, script.Name, script.Version)
            End If
            If script.Cmd <> "" And StartStop = True Then
                modcmnds.Add(script.Cmd)
                If script.Def = "" Then
                    W(DoTranslation("No definition for command {0}.", currentLang), True, ColTypes.Neutral, script.Cmd)
                    Wdbg("{0}.Def = Nothing, {0}.Def = ""Command defined by {1}""", script.Cmd, script.Name)
                    script.Def = DoTranslation("Command defined by ", currentLang) + script.Name
                End If
                moddefs.Add(script.Cmd, script.Def)
            End If
        End If
    End Sub

    '------------------------------------------- Reloader -------------------------------------------
    Sub ReloadMods()
        'Clear all scripts, commands, and defs
        modcmnds.Clear()
        moddefs.Clear()
        scripts.Clear()

        'Stop all mods
        ParseMods(False)

        'Start all mods
        ParseMods(True)
        Dim modPath As String = paths("Mods")
        For Each modFile As String In FileIO.FileSystem.GetFiles(modPath)
            CompileCustom(modFile.Replace(modPath, ""))
        Next
    End Sub

End Module
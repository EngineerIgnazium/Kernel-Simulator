﻿
'    Kernel Simulator  Copyright (C) 2018  EoflaOE
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

Imports Microsoft.Win32
Imports System.TimeZoneInfo

Public Module TimeZones

    'TODO: Don't use registry way.
    'Time Zones in an array
    Private zoneReg As RegistryKey
    Public zones As Object()
    Public zoneTimes As New Dictionary(Of String, DateTime)

    Public Sub initTimesInZones()

        'Check if running on Unix
        If (EnvironmentOSType.Contains("Unix")) Then
            zones = TimeZoneInfo.GetSystemTimeZones.ToArray
        Else
            zoneReg = Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows NT\CurrentVersion\Time Zones")
            zones = zoneReg.GetSubKeyNames()
        End If

        'Run a cleanup in the list
        zoneTimes.Clear()

        'Adds date and time to every single time zone to the list
        For Each zone In zones
            'Workaround for Unix.
            If (EnvironmentOSType.Contains("Unix")) Then
                zoneTimes.Add(zone.DisplayName, ConvertTime(KernelDateTime, FindSystemTimeZoneById(zone.DisplayName)))
            Else
                zoneTimes.Add(zone, ConvertTime(KernelDateTime, FindSystemTimeZoneById(zone)))
            End If
        Next

    End Sub

    Public Sub showTimesInZones(Optional ByVal zone As String = "all")

        For Each timezone In zones
            If (EnvironmentOSType.Contains("Unix")) Then
                If (timezone.DisplayName = zone) Then
                    Wln("- Time of {0}: {1}", "neutralText", zone, zoneTimes(zone).ToString())
                End If
            ElseIf (EnvironmentOSType.Contains("Windows") And timezone = zone) Then
                Wln("- Time of {0}: {1}", "neutralText", zone, zoneTimes(zone).ToString())
            End If
        Next
        If (zone = "all") Then
            For Each timezone In zoneTimes.Keys
                Wln("- Time of {0}: {1}", "neutralText", timezone, zoneTimes(timezone).ToString())
            Next
        End If

    End Sub

End Module

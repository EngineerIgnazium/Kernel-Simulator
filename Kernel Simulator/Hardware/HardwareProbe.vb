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

Public Module HardwareProbe

    'These are used to check to see if probing specific hardware is done.
    Private CPUDone As Boolean = False
    Private RAMDone As Boolean = False
    Private HDDDone As Boolean = False
    Private GPUDone As Boolean = False
    Private BIOSDone As Boolean = False

    'TODO: Re-write in Beta
    Public Sub ProbeHW(Optional ByVal QuietHWProbe As Boolean = False, Optional ByVal KernelUser As Char = CChar("U"))

        Wdbg("QuietHWProbe = {0}, KernelUser = {1}, ProbeFlag = {2}.", True, QuietHWProbe, KernelUser, ProbeFlag)
        If (QuietHWProbe = False) Then
            If (KernelUser = "K") Then
                If (ProbeFlag = True) Then
                    Wln("hwprobe: Your hardware will be probed. Please wait...", "neutralText")
                    Cpuinfo()
                    If (CPUDone = True) Then
                        Wdbg("CPU probed.", True, KernelVersion)
                        Wln("hwprobe: CPU: {0} {1}MHz", "neutralText", Cpuname, Cpuspeed)
                    Else
                        Wdbg("CPU failed to probe.", True, KernelVersion)
                        Wln("hwprobe: CPU: failed to probe", "neutralText")
                    End If
                    SysMemory()
                    If (RAMDone = True) Then
                        Wdbg("RAM probed.", True, KernelVersion)
                        Wln("hwprobe: RAM: {0} = {1}MB", "neutralText", SysMem, String.Join("MB + ", Capacities))
                    Else
                        Wdbg("RAM failed to probe.", True, KernelVersion)
                        Wln("hwprobe: RAM: failed to probe", "neutralText")
                    End If
                    Hddinfo()
                    If (HDDDone = True) Then
                        Wdbg("HDD probed.", True, KernelVersion)
                    Else
                        Wdbg("HDD failed to probe.", True, KernelVersion)
                        Wln("hwprobe: HDD: failed to probe", "neutralText")
                    End If
                    ProbeGPU(True, False)
                    If (GPUDone = True) Then
                        Wdbg("GPU probed.", True, KernelVersion)
                    Else
                        Wdbg("GPU failed to probe.", True, KernelVersion)
                        Wln("hwprobe: GPU: failed to probe", "neutralText")
                    End If
                    BiosInformation()
                    If (BIOSDone = True) Then
                        Wdbg("BIOS probed.", True, KernelVersion)
                        Wln("hwprobe: BIOS: {0} {1} {2} {3}", "neutralText", BIOSMan, BIOSCaption, BIOSVersion, BIOSSMBIOSVersion)
                    Else
                        Wdbg("BIOS failed to probe.", True, KernelVersion)
                        Wln("hwprobe: BIOS: failed to probe", "neutralText")
                    End If
                Else
                    Wln("hwprobe: Hardware is not probed. Probe using 'hwprobe'", "neutralText")
                End If
            ElseIf (KernelUser = "U") Then
                If (ProbeFlag = False) Then
                    Wln("hwprobe: Your hardware will be probed. Please wait...", "neutralText")
                    Cpuinfo()
                    If (CPUDone = True) Then
                        Wdbg("CPU probed.", True, KernelVersion)
                        Wln("hwprobe: CPU: {0} {1}MHz", "neutralText", Cpuname, Cpuspeed)
                    Else
                        Wdbg("CPU failed to probe.", True, KernelVersion)
                        Wln("hwprobe: CPU: failed to probe", "neutralText")
                    End If
                    SysMemory()
                    If (RAMDone = True) Then
                        Wdbg("RAM probed.", True, KernelVersion)
                        Wln("hwprobe: RAM: {0} = {1}MB", "neutralText", SysMem, String.Join("MB + ", Capacities))
                    Else
                        Wdbg("RAM failed to probe.", True, KernelVersion)
                        Wln("hwprobe: RAM: failed to probe", "neutralText")
                    End If
                    Hddinfo()
                    If (HDDDone = True) Then
                        Wdbg("HDD probed.", True, KernelVersion)
                    Else
                        Wdbg("HDD failed to probe.", True, KernelVersion)
                        Wln("hwprobe: HDD: failed to probe", "neutralText")
                    End If
                    ProbeGPU(True, False)
                    If (GPUDone = True) Then
                        Wdbg("GPU probed.", True, KernelVersion)
                    Else
                        Wdbg("GPU failed to probe.", True, KernelVersion)
                        Wln("hwprobe: GPU: failed to probe", "neutralText")
                    End If
                    BiosInformation()
                    If (BIOSDone = True) Then
                        Wdbg("BIOS probed.", True, KernelVersion)
                        Wln("hwprobe: BIOS: {0} {1} {2} {3}", "neutralText", BIOSMan, BIOSCaption, BIOSVersion, BIOSSMBIOSVersion)
                    Else
                        Wdbg("BIOS failed to probe.", True, KernelVersion)
                        Wln("hwprobe: BIOS: failed to probe", "neutralText")
                    End If
                    ProbeFlag = True
                Else
                    Wln("hwprobe: Hardware already probed.", "neutralText")
                End If
            End If
        ElseIf (QuietHWProbe = True) Then
            If (ProbeFlag = True) Then
                Cpuinfo()
                SysMemory(True)
                Hddinfo(True)
                ProbeGPU(True, True)
                BiosInformation()
            End If
        End If

    End Sub

    Public Sub ListDrivers()

        Wln("CPU: {0} {1}MHz", "neutralText", Cpuname, Cpuspeed)
        Wln("RAM: Used slots (by names): {0}", "neutralText", slotsUsedName)
        Wln("RAM: Used slots (by numbers): {0} / {1} ({2}%)", "neutralText", CStr(slotsUsedNum), totalSlots, FormatNumber(CStr(slotsUsedNum * 100 / totalSlots), 1))
        Wln("RAM: {0}", "neutralText", StatusesRAM)
        Wln("RAM: {0} = {1}MB", SysMem, String.Join("MB + ", "neutralText", Capacities))
        Wln("hwprobe: RAM: Probing status is deprecated and will be removed in future release.", "neutralText")
        Hddinfo(False, False)
        ProbeGPU(False)
        BiosInformation()

    End Sub

    'TODO: Re-organize parsers as one sub.

    Public Sub ProbeGPU(Optional ByVal KernelMode As Boolean = True, Optional ByVal QuietMode As Boolean = False)

        Try
            If ProbeFlag = True Then
                GPUDone = True
                Dim colGPUs As Object
                Dim oGPU As Object

                colGPUs = GetObject("Winmgmts:").ExecQuery("SELECT * FROM Win32_VideoController")
                Wdbg("Object created = {0}.Win32_VideoController", True, colGPUs)

                For Each oGPU In colGPUs
                    Wdbg("GPU Object = {0}.Win32_VideoController.Caption = {1}, {0}.Win32_VideoController.AdapterRAM = {2}", True, oGPU, oGPU.Caption, oGPU.AdapterRAM)
                    If (QuietMode = False And KernelMode = True) Then
                        Wln("hwprobe: GPU: {0} {1}MB", "neutralText", oGPU.Caption, CStr(oGPU.AdapterRAM / 1024 / 1024))
                    ElseIf (QuietMode = False And KernelMode = False) Then
                        Wln("GPU: {0} {1}MB", "neutralText", oGPU.Caption, CStr(oGPU.AdapterRAM / 1024 / 1024))
                    End If
                Next
            End If
        Catch ex As Exception
            GPUDone = False
            If (DebugMode = True) Then
                Wln(ex.StackTrace, "uncontError") : Wdbg(ex.StackTrace, True)
            End If
        End Try

    End Sub

    Public Sub Hddinfo(Optional ByVal QuietMode As Boolean = False, Optional ByVal KernelMode As Boolean = True)
        Try
            HDDDone = True
            Dim HDDSet As Object                                                            'Sets of hard drive
            Dim Hdd As Object                                                               'Needed to get model and size of hard drive.
            HDDSet = GetObject("Winmgmts:").ExecQuery("SELECT * FROM Win32_DiskDrive")      'it gets Winmgmts: to SELECT * FROM Win32_DiskDrive
            Wdbg("Object created = {0}.Win32_DiskDrive", True, HDDSet)
            For Each Hdd In HDDSet
                Hddmodel = CStr(Hdd.Model)
                Hddsize = CStr(Hdd.Size) / 1024 / 1024 / 1024                               'Calculate size from Hard Drive in GB
                Dim IntType As String = Hdd.InterfaceType
                Dim Status As String = Hdd.Status
                Dim Man As String = Hdd.Manufacturer
                Dim C As UInt64 = Hdd.TotalCylinders
                Dim H As UInt32 = Hdd.TotalHeads
                Dim S As UInt64 = Hdd.TotalSectors
                Wdbg("HDD Object = {0}.Win32_DiskDrive.Manufacturer = {3}, {0}.Win32_DiskDrive.Model = {1}, {0}.Win32_DiskDrive.Size = {2}, " + _
                     "{0}.Win32_DiskDrive.InterfaceType = {4}, {0}.Win32_DiskDrive.Status = {5}, CHS: {6}, {7}, {8}", True, Hdd, Hddmodel, Hddsize, Man, _
                     IntType, Status, C, H, S)
                If (QuietMode = False And Hddsize <> Nothing And Hddmodel <> Nothing And KernelMode = True) Then
                    If (Man = "(Standard disk drives)") Then
                        Wln("hwprobe: HDD: {0} {1}GB" + vbNewLine + _
                            "hwprobe: HDD: Type: {2} | Status: {3}" + vbNewLine + _
                            "hwprobe: HDD: CHS: {4} cylinders | {5} heads | {6} sectors", "neutralText", _
                            Hddmodel, FormatNumber(Hddsize, 2), IntType, Status, C, H, S)
                    Else
                        Wln("hwprobe: HDD: {0} {1} {2}GB" + vbNewLine + _
                            "hwprobe: HDD: Type: {3} | Status: {4}" + vbNewLine + _
                            "hwprobe: HDD: CHS: {5} cylinders | {6} heads | {7} sectors", "neutralText", _
                            Man, Hddmodel, FormatNumber(Hddsize, 2), IntType, Status, C, H, S)
                    End If
                    Wln("hwprobe: HDD: Probing status is deprecated and will be removed in future release.", "neutralText")
                    If (Status = "Degraded") Then
                        KernelError(CChar("C"), False, 0, "Hard drive {0} is degraded. Backup and replace before failure.", Hddmodel)
                    ElseIf (Status = "Pred Fail") Then
                        KernelError(CChar("C"), False, 0, "Hard drive {0} will predict failure in the future and you're in risk now. Backup and replace before failure.", Hddmodel)
                    End If
                ElseIf (QuietMode = False And Hddsize <> Nothing And Hddmodel <> Nothing And KernelMode = False) Then
                    If (Man = "(Standard disk drives)") Then
                        Wln("HDD: {0} {1}GB" + vbNewLine + _
                            "HDD: Type: {2} | Status: {3}" + vbNewLine + _
                            "HDD: CHS: {4} cylinders | {5} heads | {6} sectors", "neutralText", _
                            Hddmodel, FormatNumber(Hddsize, 2), IntType, Status, C, H, S)
                    Else
                        Wln("HDD: {0} {1} {2}GB" + vbNewLine + _
                            "HDD: Type: {3} | Status: {4}" + vbNewLine + _
                            "HDD: CHS: {5} cylinders | {6} heads | {7} sectors", "neutralText", _
                            Man, Hddmodel, FormatNumber(Hddsize, 2), IntType, Status, C, H, S)
                    End If
                    Wln("HDD: Probing status is deprecated and will be removed in future release.", "neutralText")
                End If
            Next
        Catch ex As Exception
            HDDDone = False
            If (DebugMode = True) Then
                Wln(ex.StackTrace, "uncontError") : Wdbg(ex.StackTrace, True)
            End If
        End Try

    End Sub

    Public Sub Cpuinfo()
        Try
            CPUDone = True
            Dim CPUSet As Object                                                            'Sets of CPU
            Dim CPU As Object                                                               'Needed to get name and clock speed of CPU
            CPUSet = GetObject("Winmgmts:").ExecQuery("SELECT * FROM Win32_Processor")      'it gets Winmgmts: to SELECT * FROM Win32_Processor
            Wdbg("Object created = {0}.Win32_Processor", True, CPUSet)
            For Each CPU In CPUSet
                Cpuname = CStr(CPU.Name)                                                    'Get name of CPU
                Cpuspeed = CStr(CPU.CurrentClockSpeed)                                      'Get name of clock speed
                Wdbg("CPU Object = {0}.Win32_Processor.Name = {1}, {0}.Win32_Processor.CurrentClockSpeed = {2}", True, CPU, Cpuname, Cpuspeed)
            Next
        Catch ex As Exception
            CPUDone = False
            If (DebugMode = True) Then
                Wln(ex.StackTrace, "uncontError") : Wdbg(ex.StackTrace, True)
            End If
        End Try
    End Sub

    Public Sub SysMemory(Optional ByVal QuietMode As Boolean = False)
        Try
            RAMDone = True
            Dim oInstance As Object
            Dim colInstances As Object
            Dim dRam As Double
            Dim colSlots As Object
            Dim oSlot As Object
            Dim temp = ""
            Dim status
            colInstances = GetObject("winmgmts:").ExecQuery("SELECT * FROM Win32_PhysicalMemory")
            colSlots = GetObject("winmgmts:").ExecQuery("SELECT * FROM Win32_PhysicalMemoryArray")
            Wdbg("colInstances = {0}.Win32_PhysicalMemory, colSlots = {1}.Win32_PhysicalMemoryArray", True, colInstances, colSlots)
            For Each oInstance In colInstances
                dRam = dRam + oInstance.Capacity                                            'Calculate RAM in bytes
                If (slotProbe = True) Then
                    slotsUsedName = slotsUsedName + oInstance.DeviceLocator + " "               'Get used slots
                    status = oInstance.Status
                    If (status.ToString = "") Then
                        StatusesRAM = StatusesRAM + oInstance.DeviceLocator + " status: Unknown | "
                    Else
                        StatusesRAM = StatusesRAM + oInstance.DeviceLocator + " status: " + status + " | "
                    End If
                End If
                temp = temp + CStr(oInstance.Capacity / 1024 / 1024) + " "
                Wdbg("oInstance = {0}.Win32_PhysicalMemory.Capacity = {1}, Total = {2}", True, oInstance, oInstance.Capacity, dRam)
            Next
            StatusesRAM = StatusesRAM.Remove(StatusesRAM.Length - 3)
            If (StatusesRAM.Contains("Unknown")) Then
                Wdbg("One or more of your chips cannot get its status, so ""unknown"" assumed.", True)
            End If
            Capacities = temp.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
            If (slotProbe = True) Then
                slotsUsedNum = Capacities.Count()
                For Each oSlot In colSlots
                    totalSlots = oSlot.MemoryDevices
                    Wdbg("oSlot = {0}.Win32_PhysicalMemoryArray.MemoryDevices | totalSlots = {1}", True, oSlot, totalSlots)
                Next
                If (QuietMode = False And slotProbe = True) Then
                    Wln("hwprobe: RAM: Used slots (by names): {0}", "neutralText", slotsUsedName)
                    Wln("hwprobe: RAM: Used slots (by numbers): {0} / {1} ({2}%)", "neutralText", CStr(slotsUsedNum), totalSlots, FormatNumber(CStr(slotsUsedNum * 100 / totalSlots), 1))
                    Wln("hwprobe: RAM: {0}", "neutralText", StatusesRAM)
                    Wln("hwprobe: RAM: Probing status is deprecated and will be removed in future release.", "neutralText")
                End If
            End If
            SysMem = dRam / 1024 / 1024 & "MB"                                              'Calculate RAM in MB
        Catch ex As Exception
            RAMDone = False
            If (DebugMode = True) Then
                Wln(ex.StackTrace, "uncontError") : Wdbg(ex.StackTrace, True)
            End If
        End Try
    End Sub

    Public Sub BiosInformation()
        Try
            BIOSDone = True
            Dim BiosInfoSpec As Object
            Dim Info As Object = GetObject("winmgmts:").ExecQuery("Select * from Win32_BIOS")
            Wdbg("Object created = {0}.Win32_BIOS", True, Info)
            For Each BiosInfoSpec In Info
                BIOSCaption = BiosInfoSpec.Name
                BIOSMan = BiosInfoSpec.Manufacturer
                BIOSSMBIOSVersion = BiosInfoSpec.SMBIOSBIOSVersion
                BIOSVersion = BiosInfoSpec.Version
                Wdbg("Bios Object = {0}.Win32_BIOS.Name = {1}, {0}.Win32_BIOS.Manufacturer = {2}, {0}.Win32_BIOS.SMBIOSBIOSVersion = {3}, {0}.Win32_BIOS.Version = {4}, ", True, BiosInfoSpec, BIOSCaption, BIOSMan, BIOSSMBIOSVersion, BIOSVersion)
            Next
            If (BIOSSMBIOSVersion = BIOSCaption) Then
                BIOSSMBIOSVersion = Nothing
            End If
        Catch ex As Exception
            BIOSDone = False
            If (DebugMode = True) Then
                Wln(ex.StackTrace, "uncontError") : Wdbg(ex.StackTrace, True)
            End If
        End Try

    End Sub

End Module

public class Item
{
    public enum Identifications
    {
        // Level:
        // Chapter 1:
        Lvl_Chapter1_Windsock_Pole = 0,
        Lvl_Chapter1_Building_Small1 = 1,
        Lvl_Chapter1_Building_Small2 = 2,
        // Chapter 2:
        Lvl_Chapter2_Location = 3,
        Lvl_Chapter2_Shed = 4,
        Lvl_Chapter2_Helipad = 5,
        Lvl_Chapter2_Office = 6,
        Lvl_Chapter2_Hangar_Small = 7,
        // Chapter 3:
        Lvl_Chapter3_Location = 8,
        Lvl_C3_Hangar = 9,
        Lvl_C3_Terminal1 = 10,
        Lvl_C3_Terminal2 = 11,
        Lvl_C3_Tower = 12,
        // Chapter 4:
        Lvl_C4_Location = 13,
        Lvl_C4_Hangar = 14,
        Lvl_C4_Radar = 15,
        Lvl_C4_Terminal = 16,
        Lvl_C4_Tower = 17,
        // Chapter 5:
        Lvl_C5_Location = 18,
        Lvl_C5_ParkingLot = 19,
        Lvl_C5_Terminal = 20,
        Lvl_C5_Gangway = 21,
        Lvl_C5_Fence = 22,
        Lvl_C5_Tower = 23,
        // Chapter 6:
        Lvl_C6_Location = 24,
        Lvl_C6_Barrack = 25,
        Lvl_C6_Walls = 26,
        Lvl_C6_HangerSmall = 27,
        Lvl_C6_GuardTower = 28,
        Lvl_C6_Tower = 29,
        // Chapter 7:
        Lvl_C7_Location = 30,
        Lvl_C7_Hangars = 31,
        Lvl_C7_Terminal = 32,
        Lvl_C7_ParkingLot = 33,
        Lvl_C7_Gateways = 34,
        Lvl_C7_Tower = 35,
        // Chapter 8:
        Lvl_C8_Location = 36,
        Lvl_C8_1 = 37,
        Lvl_C8_2 = 38,
        Lvl_C8_3 = 39,
        Lvl_C8_4 = 40,
        Lvl_C8_5 = 41,
        // Chapter 9:
        Lvl_C9_Location = 42,

        // Idle
        // Chapter 1:
        Idle_C1_Maintenance = 1000,
        Idle_C1_Pilot = 1001,
        // Chapter 2:
        Idle_C2_MaintenanceBop = 1002,
        Idle_C2_PilotCullen = 1003,
        Idle_C2_OfficeKelly = 1004,
        Idle_C2_HangarHank = 1005,
        // Chapter 3:
        Idle_C3_Maintenance_Allan = 1006,
        Idle_C3_Employee_Blake = 1007,
        Idle_C3_Employee_Eli = 1008,
        Idle_C3_Pilot_Philip = 1009,
        // Chapter 4:
        Idle_C4_HangarHomer = 1010,
        Idle_C4_EmployeeErik = 1011,
        Idle_C4_AirHostessHillary = 1012,
        Idle_C4_PilotCalden = 1013,
        // Chapter 5:
        Idle_C5_EmployeeJames = 1014,
        Idle_C5_PilotJohn = 1015,
        Idle_C5_AirHostessRita = 1016,
        Idle_C5_PilotDavid = 1017,
        // Chapter 6:
        Idle_C6_MilitaryMaintenance = 1018,
        Idle_C6_MilitaryPilot1 = 1019,
        Idle_C6_MilitaryPilot2 = 1020,
        Idle_C6_MilitaryPilot3 = 1021,
        // Chapter 7:
        Idle_C7_EmployeeDonald = 1022,
        Idle_C7_PilotBart = 1023,
        Idle_C7_AirHostessMary = 1024,
        Idle_C7_PilotRobert = 1025,
        // Chapter 8:
        Idle_C8_1 = 1026,
        Idle_C8_2 = 1027,
        Idle_C8_3 = 1028,
        Idle_C8_4 = 1029,

        // Tap
        // Chapter 1:
        Tap_C1_SmallPlane1 = 2000,
        Tap_C1_SmallPlane2 = 2001,
        // Chapter 2:
        Tap_C2_SmallPlane3 = 2002,
        Tap_C2_SmallPlane4 = 2003,
        Tap_C2_SmallHelicopter1 = 2004,
        Tap_C2_SmallHelicopter2 = 2005,
        // Chapter 3:
        Tap_C3_Jet04 = 2006,
        Tap_C3_Jet05 = 2007,
        Tap_C3_Jet06 = 2008,
        Tap_C3_Jet07 = 2009,
        // Chapter 4:
        Tap_C4_Propellor1 = 2010,
        Tap_C4_Propellor2 = 2011,
        Tap_C4_Propellor3 = 2012,
        Tap_C4_Propellor4 = 2013,
        // Chapter 5:
        Tap_C5_AirportBus = 2014,
        Tap_C5_Plane1  = 2015,
        Tap_C5_Plane2 = 2016,
        Tap_C5_Plane3 = 2017,
        // Chapter 6:
        Tap_C6_Military_SmallHeli = 2018,
        Tap_C6_Military_BigHeli = 2019,
        Tap_C6_Military_SmallFighter = 2020,
        Tap_C6_Military_BigFighter = 2021,
        // Chapter 7:
        Tap_C7_Plane1 = 2022,
        Tap_C7_Plane2 = 2023,
        Tap_C7_Plane3 = 2024,
        Tap_C7_Plane4 = 2025,
        // Chapter 8:
        Tap_C8_1 = 2026,
        Tap_C8_2 = 2027,
        Tap_C8_3 = 2028,
        Tap_C8_4 = 2029,
    }

    public const int IdleIdStart = (int)Identifications.Idle_C1_Maintenance;
    public const int IdleIdEnd = (int)Identifications.Idle_C8_4;

    public const int TapIdStart = (int)Identifications.Tap_C1_SmallPlane1;
    public const int TapIdEnd = (int)Identifications.Tap_C8_4;

    public const int LvlIdStart = (int)Identifications.Lvl_Chapter1_Windsock_Pole;
    public const int LvlIdEnd = (int)Identifications.Lvl_C8_5;
    public const int MaxLevel = (int)Identifications.Lvl_C9_Location;


}

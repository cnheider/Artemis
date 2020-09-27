using System;
using System.ComponentModel;
using Artemis.Core.DataModelExpansions;
using Artemis.Plugins.Modules.Games.R3E.API;
using Artemis.Plugins.Modules.Games.R3E.API.Data;

namespace Artemis.Plugins.Modules.Games.R3E.DataModels
{
    public class R3EDataModel : DataModel
    {
        private readonly DataContainer<Shared> _data = new DataContainer<Shared>();
        internal Shared Data
        {
            get => _data.Data;
            set => _data.Data = value;
        }

        public R3EDataModel()
        {
            Game = new R3EDataModelGame(_data);
            Session = new R3EDataModelSession(_data);
            Flags = new R3EDataModelFlags(_data);
            Car = new R3EDataModelCar(_data);
            Pit = new R3EDataModelPit(_data);
            Scoring = new R3EDataModelScoring(_data);
            Penalties = new R3EDataModelPenalties(_data);
        }

        [DataModelProperty(Name = "Game", Description = "Infos about the game-state")]
        private R3EDataModelGame Game { get; }

        [DataModelProperty(Name = "Session", Description = "Infos about the current session")]
        private R3EDataModelSession Session { get; }

        [DataModelProperty(Name = "Flags", Description = "Infos about flags")]
        private R3EDataModelFlags Flags { get; }

        [DataModelProperty(Name = "Car", Description = "Infos about the state of the car")]
        private R3EDataModelCar Car { get; }

        [DataModelProperty(Name = "Pit", Description = "Infos about the pit")]
        private R3EDataModelPit Pit { get; }

        [DataModelProperty(Name = "Scoring", Description = "Infos about the current player position etc.")]
        private R3EDataModelScoring Scoring { get; }

        [DataModelProperty(Name = "Scoring", Description = "Infos about penalties")]
        private R3EDataModelPenalties Penalties { get; }
    }

    public class R3EDataModelGame
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelGame(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Paused", Description = "Whether the game is currently paused")]
        public bool GamePaused => Data.GamePaused == 1;

        [DataModelProperty(Name = "In menu", Description = "Whether the game is currently in menu")]
        public bool GameInMenus => Data.GameInMenus == 1;

        [DataModelProperty(Name = "In replay", Description = "Whether the game is currently playing a replay")]
        public bool GameInReplay => Data.GameInReplay == 1;

        [DataModelProperty(Name = "Using VR", Description = "Whether the game is using VR")]
        public bool GameUsingVr => Data.GameUsingVr == 1;
    }

    public class R3EDataModelSession
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelSession(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Session type", Description = "Which session the player is in (practice, qualifying, race, etc.)")]
        public SessionType SessionType => (SessionType)Data.SessionType;

        [DataModelProperty(Name = "Session iteration", Description = "The current iteration of the current type of session (second qualifying session, etc.)")]
        public int SessionIteration => Data.SessionIteration;

        [DataModelProperty(Name = "Session phase", Description = "Which phase the current session is in (gridwalk, countdown, green flag, etc.)")]
        public SessionPhaseType SessionPhase => (SessionPhaseType)Data.SessionPhase;

        [DataModelProperty(Name = "Start lights", Description = "Which phase start lights are in; -1 = unavailable, 0 = off, 1-5 = redlight on and counting down, 6 = greenlight on")]
        public int StartLights => Data.StartLights;

        [DataModelProperty(Name = "Pit speed limit", Description = "The current pit speed limit in meters per second (m/s).")]
        public float SessionPitSpeedLimit => Data.SessionPitSpeedLimit;
    }

    public class R3EDataModelFlags
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelFlags(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Yellow", Description = "Whether yellow flag is currently active")]
        public bool Yellow => Data.Flags.Yellow == 1;

        [DataModelProperty(Name = "Yellow caused by player", Description = "Whether yellow flag was caused by current slot")]
        public bool YellowCausedIt => Data.Flags.YellowCausedIt == 1;

        [DataModelProperty(Name = "Yellow overtake allowed", Description = "Whether overtake of car in front by current slot is allowed under yellow flag")]
        public bool YellowOvertake => Data.Flags.YellowOvertake == 1;

        [DataModelProperty(Name = "Yellow illegal position gain", Description = "Whether you have gained positions illegaly under yellow flag to give back")]
        public bool YellowPositionsGained => Data.Flags.YellowOvertake == 1;

        [DataModelProperty(Name = "Yellow in sector 1", Description = "Whether yellow flag is currently active in sector 1")]
        public bool YellowYector1 => Data.Flags.SectorYellow.Sector1 == 1;

        [DataModelProperty(Name = "Yellow in sector 2", Description = "Whether yellow flag is currently active in sector 2")]
        public bool YellowYector2 => Data.Flags.SectorYellow.Sector2 == 1;

        [DataModelProperty(Name = "Yellow in sector 3", Description = "Whether yellow flag is currently active in sector 3")]
        public bool YellowYector3 => Data.Flags.SectorYellow.Sector3 == 1;

        [DataModelProperty(Name = "Distance to closest yellow flag", Description = "Distance (in meters) into track for closest yellow, -1.0 if no yellow flag exists")]
        public float ClosestYellowDistanceIntoTrack => Data.Flags.ClosestYellowDistanceIntoTrack;

        [DataModelProperty(Name = "Blue", Description = "Whether blue flag is currently active")]
        public bool Blue => Data.Flags.Blue == 1;

        [DataModelProperty(Name = "Black", Description = "Whether black flag is currently active")]
        public bool Black => Data.Flags.Black == 1;

        [DataModelProperty(Name = "Green", Description = "Whether green flag is currently active")]
        public bool Green => Data.Flags.Green == 1;

        [DataModelProperty(Name = "Checkered", Description = "Whether checkered flag is currently active")]
        public bool Checkered => Data.Flags.Checkered == 1;

        [DataModelProperty(Name = "White", Description = "Whether white flag is currently active")]
        public bool White => Data.Flags.White == 1;

        [DataModelProperty(Name = "Black and white", Description = "Whether black and white flag is currently active and reason")]
        public bool BlackAndWhite => Data.Flags.BlackAndWhite == 1;
    }

    public class R3EDataModelCar
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelCar(DataContainer<Shared> data)
        {
            this._data = data;

            Tires = new R3EDataModelTires(_data);
            Damage = new R3EDataModelDamage(_data);
            Velocity = new R3EDataModelVelocity(_data);
            Acceleration = new R3EDataModelAcceleration(_data);
            Orientation = new R3EDataModelOrientation(_data);
            Rotation = new R3EDataModelRotation(_data);
            AngularAcceleration = new R3EDataModelAngularAcceleration(_data);
            AngularVelocity = new R3EDataModelAngularVelocity(_data);
            GForce = new R3EDataModelGForce(_data);
        }

        [DataModelProperty(Name = "Speed", Description = "Current car speed in m/s")]
        public float Speed => Data.CarSpeed;

        [DataModelProperty(Name = "Engine RPM", Description = "Current engine RPM")]
        public float EngineRpm => Utilities.RpsToRpm(Data.EngineRps);

        [DataModelProperty(Name = "Max engine RPM", Description = "Maximum engine RPM")]
        public float MaxEngineRpm => Utilities.RpsToRpm(Data.MaxEngineRps);

        [DataModelProperty(Name = "Upshift engine RPM", Description = "Recommended upshift engine RPM")]
        public float UpshiftRpm => Utilities.RpsToRpm(Data.UpshiftRps);

        [DataModelProperty(Name = "Current gear", Description = "Current gear (0 = neutral, -1 = reverse)")]
        public int CurrentGear => Data.Gear;

        [DataModelProperty(Name = "Number of gears", Description = "Number of gears")]
        public int NumGears => Data.NumGears;

        [DataModelProperty(Name = "Total mass", Description = "Current mass of the car (including fuel)")]
        public float TotalMass => Data.TotalMass;

        [DataModelProperty(Name = "Fuel left", Description = "The amount of fuel left in lites")]
        public float FuelLeft => Data.FuelLeft;

        [DataModelProperty(Name = "Fuel capacity", Description = "The maximum amount of fuel in lites")]
        public float FuelCapacity => Data.FuelCapacity;

        [DataModelProperty(Name = "Fuel per lap", Description = "The estimated fuel usage per lap in liters")]
        public float FuelPerLap => Data.FuelPerLap;

        [DataModelProperty(Name = "Engine water temperature", Description = "Current engine water temperature")]
        public float EngineWaterTemp => Data.EngineWaterTemp;

        [DataModelProperty(Name = "Engine oil temperature", Description = "Current engine oil temperature")]
        public float EngineOilTemp => Data.EngineOilTemp;

        [DataModelProperty(Name = "Fuel pressure", Description = "Current fuel pressure")]
        public float FuelPressure => Data.FuelPressure;

        [DataModelProperty(Name = "Engine oil pressure", Description = "Current engine oil pressure")]
        public float EngineOilPressure => Data.EngineOilPressure;

        [DataModelProperty(Name = "Boost", Description = "Current boost in bar")]
        public float TurboPressure => Data.TurboPressure;

        [DataModelProperty(Name = "Throttle", Description = "Current throttle pedal usage (0.0 - 1.0)")]
        public float Throttle => Data.Throttle;

        [DataModelProperty(Name = "Brake", Description = "Current brake pedal usage (0.0 - 1.0)")]
        public float Brake => Data.Brake;

        [DataModelProperty(Name = "Clutch", Description = "Current clutch pedal usage (0.0 - 1.0)")]
        public float Clutch => Data.Clutch;

        [DataModelProperty(Name = "Steer input", Description = "Current steering wheel input in degrees")]
        public float SteerInputRaw => Data.SteerInputRaw;

        [DataModelProperty(Name = "Steer lock", Description = "Degrees to steer lock (center to full lock)")]
        public float SteerLockDegrees => Data.SteerLockDegrees;

        [DataModelProperty(Name = "Steering Force", Description = "Steering force percentage coming through steering bars")]
        public double SteeringForce => Data.Player.SteeringForce;

        [DataModelProperty(Name = "Engine Torque", Description = "Current engine torque")]
        public double EngineTorque => Data.Player.EngineTorque;

        [DataModelProperty(Name = "Down Force", Description = "Current downforce in newtons (N)")]
        public double Downforce => Data.Player.CurrentDownforce;

        [DataModelProperty(Name = "Brake bias", Description = "How much the vehicle's brakes are biased towards the back wheels (0.3 = 30%)")]
        public float BrakeBias => Data.BrakeBias;

        [DataModelProperty(Name = "ABS", Description = "Current state of the ABS")]
        public AidSettingType Abs => (AidSettingType)Data.AidSettings.Abs;

        [DataModelProperty(Name = "TC", Description = "Current state of the traction control")]
        public AidSettingType Tc => (AidSettingType)Data.AidSettings.Tc;

        [DataModelProperty(Name = "ESP", Description = "Current state of the ESP")]
        public EspAidSettingType Esp => (EspAidSettingType)Data.AidSettings.Esp;

        [DataModelProperty(Name = "Countersteer helper", Description = "Current state of the countersteer helper")]
        public AidSettingType Countersteer => (AidSettingType)Data.AidSettings.Countersteer;

        [DataModelProperty(Name = "Cornering helper", Description = "Current state of the cornering helper")]
        public AidSettingType Cornering => (AidSettingType)Data.AidSettings.Cornering;

        [DataModelProperty(Name = "DRS allowed", Description = "Whether DRS is allowed")]
        public bool DRSAllowed => Data.Drs.Equipped == 1;

        [DataModelProperty(Name = "DRS engaged", Description = "Whether DRS is currently engaged")]
        public bool DRSEngaged => Data.Drs.Engaged == 1;

        [DataModelProperty(Name = "DRS activations left", Description = "Number of remaining DRS activations")]
        public int DRSActivationsLeft => Data.Drs.NumActivationsLeft;

        [DataModelProperty(Name = "Push to pass allowed", Description = "Whether push to pass is allowed")]
        public bool PTPAllowed => Data.PushToPass.Available == 1;

        [DataModelProperty(Name = "Push to pass engaged", Description = "Whether push to pass is currently engaged")]
        public bool PTPEngaged => Data.PushToPass.Engaged == 1;

        [DataModelProperty(Name = "Push to pass activations left", Description = "Number of remaining push to pass activations")]
        public int PTPActivationsLeft => Data.PushToPass.AmountLeft;

        [DataModelProperty(Name = "Tires", Description = "Infos about the tires")]
        public R3EDataModelTires Tires { get; }

        [DataModelProperty(Name = "Damage", Description = "Infos about the damage of the car")]
        public R3EDataModelDamage Damage { get; }

        [DataModelProperty(Name = "Velocity", Description = "Car velocity in meter per second (m/s)")]
        public R3EDataModelVelocity Velocity { get; }

        [DataModelProperty(Name = "Acceleration", Description = "Car acceleration in meter per second squared (m/s^2)")]
        public R3EDataModelAcceleration Acceleration { get; }

        [DataModelProperty(Name = "Orientation", Description = "Car body orientation in euler angles")]
        public R3EDataModelOrientation Orientation { get; }

        [DataModelProperty(Name = "Rotation", Description = "Car body rotation")]
        public R3EDataModelRotation Rotation { get; }

        [DataModelProperty(Name = "AngularAcceleration", Description = "Car body angular acceleration (torque divided by inertia)")]
        public R3EDataModelAngularAcceleration AngularAcceleration { get; }

        [DataModelProperty(Name = "AngularVelocity", Description = "Car angular velocity in radians per second")]
        public R3EDataModelAngularVelocity AngularVelocity { get; }

        [DataModelProperty(Name = "GForce", Description = "Driver g-force local to car")]
        public R3EDataModelGForce GForce { get; }
    }

    public class R3EDataModelPit
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelPit(DataContainer<Shared> data)
        {
            this._data = data;

        }

        [DataModelProperty(Name = "In Pitlane", Description = "If current vehicle is in pitline")]
        public bool InPitlane => Data.InPitlane == 1;

        [DataModelProperty(Name = "Speed limit", Description = "Pit speed limit in m/s")]
        public float PitSpeedLimit => Data.SessionPitSpeedLimit;

        [DataModelProperty(Name = "Pit window status", Description = "Current status of the pit stop")]
        public PitWindowType PitWindowStatus => (PitWindowType)Data.PitWindowStatus;

        [DataModelProperty(Name = "Pit state", Description = "Current vehicle pit state")]
        public PitStateType PitState => (PitStateType)Data.PitState;

        [DataModelProperty(Name = "Pit Total Duration", Description = "Total duration of the requested pit stop")]
        public float PitTotalDuration => Data.PitTotalDuration;

        [DataModelProperty(Name = "Pit Elapsed Time", Description = "Elapsed duration of the current pit stop")]
        public float PitElapsedTime => Data.PitElapsedTime;

        [DataModelProperty(Name = "Pit action", Description = "Actions of the current pit stop")]
        public PitActionType PitAction => (PitActionType)Data.PitAction;

        [DataModelProperty(Name = "Pit stops performed", Description = "Number of pitstops the current vehicle has performed (-1 = N/A)")]
        public int NumPitstopsPerformed => Data.NumPitstopsPerformed;
    }

    public class R3EDataModelScoring
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelScoring(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Position", Description = "Current position (1 = first place)")]
        public int Position => Data.Position;

        [DataModelProperty(Name = "Position class", Description = "Current position (1 = first place) based on performance index")]
        public int PositionClass => Data.PositionClass;

        [DataModelProperty(Name = "Finish status", Description = "Current player finish status")]
        public FinishStatusType FinishStatus => (FinishStatusType)Data.FinishStatus;

        [DataModelProperty(Name = "Lap distance fraction", Description = "Fraction of lap completition (0.0 - 1.0)")]
        public float LapDistanceFraction => Data.LapDistanceFraction;

        [DataModelProperty(Name = "Lap time delta leader", Description = "The time delta between the player's time and the leader of the current session")]
        public float LapTimeDeltaLeader => Data.LapTimeDeltaLeader;

        [DataModelProperty(Name = "Lap time delta class leader", Description = "The time delta between the player's time and the leader of the player's class in the current session")]
        public float LapTimeDeltaLeaderClass => Data.LapTimeDeltaLeaderClass;

        [DataModelProperty(Name = "Tiemd elta front", Description = "Time delta between the player and the car placed in front in seconds")]
        public float TimeDeltaFront => Data.TimeDeltaFront;

        [DataModelProperty(Name = "Time delta behind", Description = "Time delta between the player and the car placed behind in seconds")]
        public float TimeDeltaBehind => Data.TimeDeltaBehind;

        [DataModelProperty(Name = "Time delta best self", Description = "Time delta between this car's current laptime and this car's best laptime")]
        public float TimeDeltaBestSelf => Data.TimeDeltaBestSelf;
    }

    public class R3EDataModelPenalties
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelPenalties(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Cut track warnings", Description = "Total number of cut track warnings (-1 = N/A)")]
        public int CutTrackWarnings => Data.CutTrackWarnings;

        [DataModelProperty(Name = "Number of pending penalties", Description = "Total number of penalties pending for the car")]
        public int NumPenalties => Data.NumPenalties;

        [DataModelProperty(Name = "Drive through", Description = "Whether there is a drive through penalty for the player")]
        public bool DriveThrough => Data.Penalties.DriveThrough == 1;

        [DataModelProperty(Name = "Stop and go", Description = "Whether there is a stop and go penalty for the player")]
        public bool StopAndGo => Data.Penalties.StopAndGo == 1;

        [DataModelProperty(Name = "Pit stop", Description = "Whether there is a pit stop penalty for the player")]
        public bool PitStop => Data.Penalties.PitStop == 1;

        [DataModelProperty(Name = "Time deduction", Description = "Whether there is a time deduction penalty for the player")]
        public bool TimeDeduction => Data.Penalties.TimeDeduction == 1;

        [DataModelProperty(Name = "Slow down", Description = "Whether there is a slow down penalty for the player")]
        public bool SlowDown => Data.Penalties.SlowDown == 1;
    }

    public class R3EDataModelVelocity
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelVelocity(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-velocity in m/s")]
        public double X => Data.Player.LocalVelocity.X;

        [DataModelProperty(Name = "Y", Description = "Y-velocity in m/s")]
        public double Y => Data.Player.LocalVelocity.Y;

        [DataModelProperty(Name = "Z", Description = "Z-velocity in m/s")]
        public double Z => Data.Player.LocalVelocity.Z;
    }

    public class R3EDataModelAcceleration
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelAcceleration(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-acceleration in m/s²")]
        public double X => Data.Player.LocalAcceleration.X;

        [DataModelProperty(Name = "Y", Description = "Y-acceleration in m/s²")]
        public double Y => Data.Player.LocalAcceleration.Y;

        [DataModelProperty(Name = "Z", Description = "Z-acceleration in m/s²")]
        public double Z => Data.Player.LocalAcceleration.Z;
    }

    public class R3EDataModelOrientation
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelOrientation(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-orientation as euler angle")]
        public double X => Data.Player.Orientation.X;

        [DataModelProperty(Name = "Y", Description = "Y-orientation as euler angle²")]
        public double Y => Data.Player.Orientation.Y;

        [DataModelProperty(Name = "Z", Description = "Z-orientation as euler angle")]
        public double Z => Data.Player.Orientation.Z;
    }

    public class R3EDataModelRotation
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelRotation(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-acceleration as euler angle")]
        public double X => Data.Player.Rotation.X;

        [DataModelProperty(Name = "Y", Description = "Y-acceleration as euler angle")]
        public double Y => Data.Player.Rotation.Y;

        [DataModelProperty(Name = "Z", Description = "Z-acceleration as euler angle")]
        public double Z => Data.Player.Rotation.Z;
    }

    public class R3EDataModelAngularAcceleration
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelAngularAcceleration(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-angular acceleration (torque divided by inertia)")]
        public double X => Data.Player.AngularAcceleration.X;

        [DataModelProperty(Name = "Y", Description = "Y-angular acceleration (torque divided by inertia)")]
        public double Y => Data.Player.AngularAcceleration.Y;

        [DataModelProperty(Name = "Z", Description = "Z-angular acceleration (torque divided by inertia)")]
        public double Z => Data.Player.AngularAcceleration.Z;
    }

    public class R3EDataModelAngularVelocity
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelAngularVelocity(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-angular velocity in radians/s")]
        public double X => Data.Player.LocalAngularVelocity.X;

        [DataModelProperty(Name = "Y", Description = "Y-angular velocity in radians/s")]
        public double Y => Data.Player.LocalAngularVelocity.Y;

        [DataModelProperty(Name = "Z", Description = "Z-angular velocity in radians/s")]
        public double Z => Data.Player.LocalAngularVelocity.Z;
    }

    public class R3EDataModelGForce
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelGForce(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "X", Description = "X-force in g")]
        public double X => Data.Player.LocalGforce.X;

        [DataModelProperty(Name = "Y", Description = "Y-force in g")]
        public double Y => Data.Player.LocalGforce.Y;

        [DataModelProperty(Name = "Z", Description = "Z-force in g")]
        public double Z => Data.Player.LocalGforce.Z;
    }

    public class R3EDataModelTires
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelTires(DataContainer<Shared> data)
        {
            this._data = data;

            FrontLeft = new R3EDataModelTire(_data, d => d.FrontLeft, t => t.FrontLeft, () => (TireType)Data.TireTypeFront, () => (TireSubtype)Data.TireSubtypeFront);
            FrontRight = new R3EDataModelTire(_data, d => d.FrontRight, t => t.FrontRight, () => (TireType)Data.TireTypeFront, () => (TireSubtype)Data.TireSubtypeFront);
            RearLeft = new R3EDataModelTire(_data, d => d.RearLeft, t => t.RearLeft, () => (TireType)Data.TireTypeRear, () => (TireSubtype)Data.TireSubtypeRear);
            RearRight = new R3EDataModelTire(_data, d => d.RearRight, t => t.RearRight, () => (TireType)Data.TireTypeRear, () => (TireSubtype)Data.TireSubtypeRear);
        }

        [DataModelProperty(Name = "Front Left", Description = "Infos about the front left tire")]
        public R3EDataModelTire FrontLeft { get; }

        [DataModelProperty(Name = "Front Right", Description = "Infos about the front right tire")]
        public R3EDataModelTire FrontRight { get; }

        [DataModelProperty(Name = "Rear Left", Description = "Infos about the rear left tire")]
        public R3EDataModelTire RearLeft { get; }

        [DataModelProperty(Name = "Rear Right", Description = "Infos about the rear right tire")]
        public R3EDataModelTire RearRight { get; }
    }

    public class R3EDataModelTire
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        private Func<TireData<float>, float> _tireData;
        private Func<TireData<TireTempInformation>, TireTempInformation> _tireTemperatureData;
        private Func<TireType> _type;
        private Func<TireSubtype> _subtype;

        internal R3EDataModelTire(DataContainer<Shared> data, Func<TireData<float>, float> tireData,
                                  Func<TireData<TireTempInformation>, TireTempInformation> tireTemperatureData,
                                  Func<TireType> type, Func<TireSubtype> subtype)
        {
            this._data = data;
            this._tireData = tireData;
            this._tireTemperatureData = tireTemperatureData;
            this._type = type;
            this._subtype = subtype;
        }

        [DataModelProperty(Name = "Tire type", Description = "The type of the tire")]
        public TireType TireType => _type();

        [DataModelProperty(Name = "Tire subtype", Description = "The subtype of the tire")]
        public TireSubtype TireSubtype => _subtype();

        [DataModelProperty(Name = "Tire speed", Description = "The current speed of the tire in km/h")]
        public float TireSpeed => Utilities.MpsToKph(_tireData(Data.TireSpeed));

        [DataModelProperty(Name = "Tire grip", Description = "The current grip of the tire (0.0 - 1.0)")]
        public float TireGrip => _tireData(Data.TireGrip);

        [DataModelProperty(Name = "Tire wear", Description = "The current wear of the tire (0.0 - 1.0)")]
        public float TireWear => _tireData(Data.TireWear);

        [DataModelProperty(Name = "Tire dirt", Description = "The current percentage of dirt on the tire (0.0 - 1.0)")]
        public float TireDirt => _tireData(Data.TireDirt);

        [DataModelProperty(Name = "Tire pressure", Description = "The current pressure of the tire in kPa")]
        public float TirePressure => _tireData(Data.TirePressure);

        [DataModelProperty(Name = "Tire load", Description = "The current load of the tire in N")]
        public float TireLoad => _tireData(Data.TireLoad);

        [DataModelProperty(Name = "Brake pressure", Description = "The current pressure of the brake on that tire in kN")]
        public float BrakePressure => _tireData(Data.BrakePressure);

        [DataModelProperty(Name = "Tire optimal temperature", Description = "The optimal temperature of the tire")]
        public float TireOptimalTemperature => _tireTemperatureData(Data.TireTemp).OptimalTemp;

        [DataModelProperty(Name = "Tire cold temperature", Description = "The temperature at which the tire is considered cold")]
        public float TireColdTemperature => _tireTemperatureData(Data.TireTemp).ColdTemp;

        [DataModelProperty(Name = "Tire hot temperature", Description = "The temperature at which the tire is considered hot")]
        public float TireHotTemperature => _tireTemperatureData(Data.TireTemp).HotTemp;

        [DataModelProperty(Name = "Tire current temperature left", Description = "The current tempreature at the left side of the tire")]
        public float TireTemperatureLeft => _tireTemperatureData(Data.TireTemp).CurrentTemp.Left;

        [DataModelProperty(Name = "Tire current temperature center", Description = "The current tempreature at the center of the tire")]
        public float TireTemperatureCenter => _tireTemperatureData(Data.TireTemp).CurrentTemp.Center;

        [DataModelProperty(Name = "Tire current temperature right", Description = "The current tempreature at the ride side of the tire")]
        public float TireTemperatureRight => _tireTemperatureData(Data.TireTemp).CurrentTemp.Right;
    }

    public class R3EDataModelDamage
    {
        private readonly DataContainer<Shared> _data;
        private Shared Data => _data.Data;

        internal R3EDataModelDamage(DataContainer<Shared> data)
        {
            this._data = data;
        }

        [DataModelProperty(Name = "Engine", Description = "The current damnage state of the engine (0.0 - 1.0; 0 = destroyed)")]
        public float EngineDamage => Data.CarDamage.Engine;

        [DataModelProperty(Name = "Transmission", Description = "The current damnage state of the transmission (0.0 - 1.0; 0 = destroyed)")]
        public float TransmissionDamage => Data.CarDamage.Transmission;

        [DataModelProperty(Name = "Aerodynamics", Description = "The current damnage state of the car aerodynamics (0.0 - 1.0; 0 = destroyed)")]
        public float AerodynamicsDamage => Data.CarDamage.Aerodynamics;

        [DataModelProperty(Name = "Suspension", Description = "The current damnage state of the suspension (0.0 - 1.0; 0 = destroyed)")]
        public float SuspensionDamage => Data.CarDamage.Suspension;
    }

    public enum SessionType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Practice")]
        Practice = 0,

        [Description("Qualify")]
        Qualify = 1,

        [Description("Race")]
        Race = 2,

        [Description("Warmup")]
        Warmup = 3,
    }

    public enum SessionPhaseType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Currently in garage")]
        Garage = 1,

        [Description("Gridwalk or track walkthrough")]
        Gridwalk = 2,

        [Description("Formation lap, rolling start etc.")]
        Formation = 3,

        [Description("Countdown to race is ongoing")]
        Countdown = 4,

        [Description("Race is ongoing")]
        Green = 5,

        [Description("End of session")]
        Checkered = 6,
    }

    public enum PitWindowType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Pit stops are not enabled for this session")]
        Disabled = 0,

        [Description("Pit stops are enabled, but you're not allowed to perform one right now")]
        Closed = 1,

        [Description("Allowed to perform a pit stop now")]
        Open = 2,

        [Description("Currently performing the pit stop changes (changing driver, etc.)")]
        Stopped = 3,

        [Description("After the current mandatory pitstop have been completed")]
        Completed = 4,
    };

    public enum PitStateType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("None")]
        None = 0,

        [Description("Requested stop")]
        Requested = 1,

        [Description("Entered pitlane heading for pitspot")]
        Entered = 2,

        [Description("Stopped at pitspot")]
        Stopped = 3,

        [Description("Exiting pitspot heading for pit exit")]
        Exiting = 4,
    };

    [Flags]
    public enum PitActionType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("None")]
        None = 0,

        [Description("Preparing")]
        Preparing = 1,

        [Description("Penalty serve")]
        Penalty = 2,

        [Description("Driver change")]
        DriverChange = 4,

        [Description("Refueling")]
        Refueling = 8,

        [Description("Front tires")]
        FrontTires = 16,

        [Description("Rear tires,")]
        RearTires = 32,

        [Description("Front wing")]
        FrontWing = 64,

        [Description("Rear wing")]
        RearWing = 128,

        [Description("Suspension")]
        Suspension = 256,
    }

    public enum FinishStatusType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Still on track, not finished")]
        None = 0,

        [Description("Finished session normally")]
        Finished = 1,

        [Description("Did not finish")]
        DNF = 2,

        [Description("Did not qualify")]
        DNQ = 3,

        [Description("Did not start")]
        DNS = 4,

        [Description("Disqualified")]
        DQ = 5,
    }

    public enum AidSettingType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Off")]
        Off = 0,

        [Description("On")]
        On = 1,

        [Description("Active")]
        CurrentlyActive = 5
    }

    public enum EspAidSettingType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Off")]
        Off = 0,

        [Description("Low")]
        Low = 1,

        [Description("Medium")]
        Medium = 2,

        [Description("High")]
        High = 3,

        [Description("Active")]
        CurrentlyActive = 5
    }

    public enum TireType
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Option")]
        Option = 0,

        [Description("Prime")]
        Prime = 1,
    };

    public enum TireSubtype
    {
        [Description("Unavailable")]
        Unavailable = -1,

        [Description("Primary")]
        Primary = 0,

        [Description("Alternate")]
        Alternate = 1,

        [Description("Soft")]
        Soft = 2,

        [Description("Medium")]
        Medium = 3,

        [Description("Hard")]
        Hard = 4
    };

    internal class DataContainer<T>
    {
        public T Data { get; set; }
    }
}
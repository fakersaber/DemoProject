// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CreateObjectInfo.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
/// <summary>Holder for reflection information generated from CreateObjectInfo.proto</summary>
public static partial class CreateObjectInfoReflection {

  #region Descriptor
  /// <summary>File descriptor for CreateObjectInfo.proto</summary>
  public static pbr::FileDescriptor Descriptor {
    get { return descriptor; }
  }
  private static pbr::FileDescriptor descriptor;

  static CreateObjectInfoReflection() {
    byte[] descriptorData = global::System.Convert.FromBase64String(
        string.Concat(
          "ChZDcmVhdGVPYmplY3RJbmZvLnByb3RvImUKDUNyZWF0ZU9iakluZm8SEwoL",
          "aXNNYW5jbGllbnQYASABKAgSEAoIUGxheWVySWQYAiABKAUSGwoIUG9zaXRp",
          "b24YAyABKAsyCS5ZVmVjdG9yMhIQCghSb3RhdGlvbhgEIAEoAiI4ChBFbmVy",
          "Z3lTcGhlcmVJbml0EiQKDUFsbFNwaGVyZVBvbGwYASADKAsyDS5FbmVyZ3lT",
          "cGhlcmUiXQoMRW5lcmd5U3BoZXJlEhAKCFBsYXllcklkGAEgASgFEhAKCFNw",
          "aGVyZUlkGAIgASgFEgwKBHR5cGUYAyABKAUSGwoIUG9zaXRpb24YBCABKAsy",
          "CS5ZVmVjdG9yMiIgCghZVmVjdG9yMhIJCgF4GAEgASgCEgkKAXkYAiABKAJi",
          "BnByb3RvMw=="));
    descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
        new pbr::FileDescriptor[] { },
        new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
          new pbr::GeneratedClrTypeInfo(typeof(global::CreateObjInfo), global::CreateObjInfo.Parser, new[]{ "IsManclient", "PlayerId", "Position", "Rotation" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::EnergySphereInit), global::EnergySphereInit.Parser, new[]{ "AllSpherePoll" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::EnergySphere), global::EnergySphere.Parser, new[]{ "PlayerId", "SphereId", "Type", "Position" }, null, null, null, null),
          new pbr::GeneratedClrTypeInfo(typeof(global::YVector2), global::YVector2.Parser, new[]{ "X", "Y" }, null, null, null, null)
        }));
  }
  #endregion

}
#region Messages
public sealed partial class CreateObjInfo : pb::IMessage<CreateObjInfo> {
  private static readonly pb::MessageParser<CreateObjInfo> _parser = new pb::MessageParser<CreateObjInfo>(() => new CreateObjInfo());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<CreateObjInfo> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CreateObjectInfoReflection.Descriptor.MessageTypes[0]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateObjInfo() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateObjInfo(CreateObjInfo other) : this() {
    isManclient_ = other.isManclient_;
    playerId_ = other.playerId_;
    position_ = other.position_ != null ? other.position_.Clone() : null;
    rotation_ = other.rotation_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public CreateObjInfo Clone() {
    return new CreateObjInfo(this);
  }

  /// <summary>Field number for the "isManclient" field.</summary>
  public const int IsManclientFieldNumber = 1;
  private bool isManclient_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool IsManclient {
    get { return isManclient_; }
    set {
      isManclient_ = value;
    }
  }

  /// <summary>Field number for the "PlayerId" field.</summary>
  public const int PlayerIdFieldNumber = 2;
  private int playerId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int PlayerId {
    get { return playerId_; }
    set {
      playerId_ = value;
    }
  }

  /// <summary>Field number for the "Position" field.</summary>
  public const int PositionFieldNumber = 3;
  private global::YVector2 position_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::YVector2 Position {
    get { return position_; }
    set {
      position_ = value;
    }
  }

  /// <summary>Field number for the "Rotation" field.</summary>
  public const int RotationFieldNumber = 4;
  private float rotation_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public float Rotation {
    get { return rotation_; }
    set {
      rotation_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as CreateObjInfo);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(CreateObjInfo other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (IsManclient != other.IsManclient) return false;
    if (PlayerId != other.PlayerId) return false;
    if (!object.Equals(Position, other.Position)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Rotation, other.Rotation)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (IsManclient != false) hash ^= IsManclient.GetHashCode();
    if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
    if (position_ != null) hash ^= Position.GetHashCode();
    if (Rotation != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Rotation);
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (IsManclient != false) {
      output.WriteRawTag(8);
      output.WriteBool(IsManclient);
    }
    if (PlayerId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(PlayerId);
    }
    if (position_ != null) {
      output.WriteRawTag(26);
      output.WriteMessage(Position);
    }
    if (Rotation != 0F) {
      output.WriteRawTag(37);
      output.WriteFloat(Rotation);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (IsManclient != false) {
      size += 1 + 1;
    }
    if (PlayerId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
    }
    if (position_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
    }
    if (Rotation != 0F) {
      size += 1 + 4;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(CreateObjInfo other) {
    if (other == null) {
      return;
    }
    if (other.IsManclient != false) {
      IsManclient = other.IsManclient;
    }
    if (other.PlayerId != 0) {
      PlayerId = other.PlayerId;
    }
    if (other.position_ != null) {
      if (position_ == null) {
        Position = new global::YVector2();
      }
      Position.MergeFrom(other.Position);
    }
    if (other.Rotation != 0F) {
      Rotation = other.Rotation;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          IsManclient = input.ReadBool();
          break;
        }
        case 16: {
          PlayerId = input.ReadInt32();
          break;
        }
        case 26: {
          if (position_ == null) {
            Position = new global::YVector2();
          }
          input.ReadMessage(Position);
          break;
        }
        case 37: {
          Rotation = input.ReadFloat();
          break;
        }
      }
    }
  }

}

public sealed partial class EnergySphereInit : pb::IMessage<EnergySphereInit> {
  private static readonly pb::MessageParser<EnergySphereInit> _parser = new pb::MessageParser<EnergySphereInit>(() => new EnergySphereInit());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<EnergySphereInit> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CreateObjectInfoReflection.Descriptor.MessageTypes[1]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphereInit() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphereInit(EnergySphereInit other) : this() {
    allSpherePoll_ = other.allSpherePoll_.Clone();
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphereInit Clone() {
    return new EnergySphereInit(this);
  }

  /// <summary>Field number for the "AllSpherePoll" field.</summary>
  public const int AllSpherePollFieldNumber = 1;
  private static readonly pb::FieldCodec<global::EnergySphere> _repeated_allSpherePoll_codec
      = pb::FieldCodec.ForMessage(10, global::EnergySphere.Parser);
  private readonly pbc::RepeatedField<global::EnergySphere> allSpherePoll_ = new pbc::RepeatedField<global::EnergySphere>();
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public pbc::RepeatedField<global::EnergySphere> AllSpherePoll {
    get { return allSpherePoll_; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as EnergySphereInit);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(EnergySphereInit other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if(!allSpherePoll_.Equals(other.allSpherePoll_)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    hash ^= allSpherePoll_.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    allSpherePoll_.WriteTo(output, _repeated_allSpherePoll_codec);
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    size += allSpherePoll_.CalculateSize(_repeated_allSpherePoll_codec);
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(EnergySphereInit other) {
    if (other == null) {
      return;
    }
    allSpherePoll_.Add(other.allSpherePoll_);
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 10: {
          allSpherePoll_.AddEntriesFrom(input, _repeated_allSpherePoll_codec);
          break;
        }
      }
    }
  }

}

public sealed partial class EnergySphere : pb::IMessage<EnergySphere> {
  private static readonly pb::MessageParser<EnergySphere> _parser = new pb::MessageParser<EnergySphere>(() => new EnergySphere());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<EnergySphere> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CreateObjectInfoReflection.Descriptor.MessageTypes[2]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphere() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphere(EnergySphere other) : this() {
    playerId_ = other.playerId_;
    sphereId_ = other.sphereId_;
    type_ = other.type_;
    position_ = other.position_ != null ? other.position_.Clone() : null;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public EnergySphere Clone() {
    return new EnergySphere(this);
  }

  /// <summary>Field number for the "PlayerId" field.</summary>
  public const int PlayerIdFieldNumber = 1;
  private int playerId_;
  /// <summary>
  ///获取或消耗的玩家id
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int PlayerId {
    get { return playerId_; }
    set {
      playerId_ = value;
    }
  }

  /// <summary>Field number for the "SphereId" field.</summary>
  public const int SphereIdFieldNumber = 2;
  private int sphereId_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int SphereId {
    get { return sphereId_; }
    set {
      sphereId_ = value;
    }
  }

  /// <summary>Field number for the "type" field.</summary>
  public const int TypeFieldNumber = 3;
  private int type_;
  /// <summary>
  ///能量球类型
  /// </summary>
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int Type {
    get { return type_; }
    set {
      type_ = value;
    }
  }

  /// <summary>Field number for the "Position" field.</summary>
  public const int PositionFieldNumber = 4;
  private global::YVector2 position_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public global::YVector2 Position {
    get { return position_; }
    set {
      position_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as EnergySphere);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(EnergySphere other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (PlayerId != other.PlayerId) return false;
    if (SphereId != other.SphereId) return false;
    if (Type != other.Type) return false;
    if (!object.Equals(Position, other.Position)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (PlayerId != 0) hash ^= PlayerId.GetHashCode();
    if (SphereId != 0) hash ^= SphereId.GetHashCode();
    if (Type != 0) hash ^= Type.GetHashCode();
    if (position_ != null) hash ^= Position.GetHashCode();
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (PlayerId != 0) {
      output.WriteRawTag(8);
      output.WriteInt32(PlayerId);
    }
    if (SphereId != 0) {
      output.WriteRawTag(16);
      output.WriteInt32(SphereId);
    }
    if (Type != 0) {
      output.WriteRawTag(24);
      output.WriteInt32(Type);
    }
    if (position_ != null) {
      output.WriteRawTag(34);
      output.WriteMessage(Position);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (PlayerId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(PlayerId);
    }
    if (SphereId != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(SphereId);
    }
    if (Type != 0) {
      size += 1 + pb::CodedOutputStream.ComputeInt32Size(Type);
    }
    if (position_ != null) {
      size += 1 + pb::CodedOutputStream.ComputeMessageSize(Position);
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(EnergySphere other) {
    if (other == null) {
      return;
    }
    if (other.PlayerId != 0) {
      PlayerId = other.PlayerId;
    }
    if (other.SphereId != 0) {
      SphereId = other.SphereId;
    }
    if (other.Type != 0) {
      Type = other.Type;
    }
    if (other.position_ != null) {
      if (position_ == null) {
        Position = new global::YVector2();
      }
      Position.MergeFrom(other.Position);
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 8: {
          PlayerId = input.ReadInt32();
          break;
        }
        case 16: {
          SphereId = input.ReadInt32();
          break;
        }
        case 24: {
          Type = input.ReadInt32();
          break;
        }
        case 34: {
          if (position_ == null) {
            Position = new global::YVector2();
          }
          input.ReadMessage(Position);
          break;
        }
      }
    }
  }

}

public sealed partial class YVector2 : pb::IMessage<YVector2> {
  private static readonly pb::MessageParser<YVector2> _parser = new pb::MessageParser<YVector2>(() => new YVector2());
  private pb::UnknownFieldSet _unknownFields;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pb::MessageParser<YVector2> Parser { get { return _parser; } }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public static pbr::MessageDescriptor Descriptor {
    get { return global::CreateObjectInfoReflection.Descriptor.MessageTypes[3]; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  pbr::MessageDescriptor pb::IMessage.Descriptor {
    get { return Descriptor; }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public YVector2() {
    OnConstruction();
  }

  partial void OnConstruction();

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public YVector2(YVector2 other) : this() {
    x_ = other.x_;
    y_ = other.y_;
    _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public YVector2 Clone() {
    return new YVector2(this);
  }

  /// <summary>Field number for the "x" field.</summary>
  public const int XFieldNumber = 1;
  private float x_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public float X {
    get { return x_; }
    set {
      x_ = value;
    }
  }

  /// <summary>Field number for the "y" field.</summary>
  public const int YFieldNumber = 2;
  private float y_;
  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public float Y {
    get { return y_; }
    set {
      y_ = value;
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override bool Equals(object other) {
    return Equals(other as YVector2);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public bool Equals(YVector2 other) {
    if (ReferenceEquals(other, null)) {
      return false;
    }
    if (ReferenceEquals(other, this)) {
      return true;
    }
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(X, other.X)) return false;
    if (!pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.Equals(Y, other.Y)) return false;
    return Equals(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override int GetHashCode() {
    int hash = 1;
    if (X != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(X);
    if (Y != 0F) hash ^= pbc::ProtobufEqualityComparers.BitwiseSingleEqualityComparer.GetHashCode(Y);
    if (_unknownFields != null) {
      hash ^= _unknownFields.GetHashCode();
    }
    return hash;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public override string ToString() {
    return pb::JsonFormatter.ToDiagnosticString(this);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void WriteTo(pb::CodedOutputStream output) {
    if (X != 0F) {
      output.WriteRawTag(13);
      output.WriteFloat(X);
    }
    if (Y != 0F) {
      output.WriteRawTag(21);
      output.WriteFloat(Y);
    }
    if (_unknownFields != null) {
      _unknownFields.WriteTo(output);
    }
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public int CalculateSize() {
    int size = 0;
    if (X != 0F) {
      size += 1 + 4;
    }
    if (Y != 0F) {
      size += 1 + 4;
    }
    if (_unknownFields != null) {
      size += _unknownFields.CalculateSize();
    }
    return size;
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(YVector2 other) {
    if (other == null) {
      return;
    }
    if (other.X != 0F) {
      X = other.X;
    }
    if (other.Y != 0F) {
      Y = other.Y;
    }
    _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
  }

  [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
  public void MergeFrom(pb::CodedInputStream input) {
    uint tag;
    while ((tag = input.ReadTag()) != 0) {
      switch(tag) {
        default:
          _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
          break;
        case 13: {
          X = input.ReadFloat();
          break;
        }
        case 21: {
          Y = input.ReadFloat();
          break;
        }
      }
    }
  }

}

#endregion


#endregion Designer generated code
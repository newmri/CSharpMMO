START ../../PacketGenerator/bin/PacketGenerator.exe ../../PacketGenerator/PDL.xml
XCOPY /Y GenPackets.cs "../../DummyClient/Packet"
XCOPY /Y GenPackets.cs "../../Server/Packet"
XCOPY /Y GameServerToClientPacketManager.cs "../../DummyClient/Packet"
XCOPY /Y ClientToGameServerPacketManager.cs "../../Server/Packet"
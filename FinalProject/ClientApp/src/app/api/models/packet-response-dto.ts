import { PacketRm } from "./packet";

export interface PacketResponseDto {
  packets: PacketRm[];
  packetId: number;
}

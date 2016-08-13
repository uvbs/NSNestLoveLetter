#include "stdafx.h"
#include <boost/bind.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/enable_shared_from_this.hpp>
#include <boost/asio.hpp>
#include "PacketProtocol.h"
#include "Packet.h"
#include "Connection.h"
#include "User.h"
#include "Server.h"


using boost::asio::ip::tcp;	

void CConnection::handle_Accept(const boost::system::error_code& err, size_t byte_transferred) {	
	m_Socket.async_read_some(
		boost::asio::buffer(m_RecvBuf),
		boost::bind(&CConnection::handle_Read, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred 
			));	
}

void CConnection::handle_Read(const boost::system::error_code& err, size_t byte_transferred) {	
	if (!err) {
		if (m_RecvBuf.size()) {			
			BYTE *pBuf = &m_RecvBuf[0];

			while (pBuf) {
				LONG nState = packetBuf.Append(pBuf, byte_transferred);
				if (nState == InPacket::PS_COMPLETE) {
					ProcessPacket(packetBuf);
					packetBuf.Clear();
				}
			}
		}

		m_Socket.async_read_some(
			boost::asio::buffer(m_RecvBuf),
			boost::bind(&CConnection::handle_Read, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred
				));
	}
	else {		
		std::cout << m_uSocketSN << " - Disconnect(Write) : " << err.message() << std::endl;
		Server_Wrapper::get_mutable_instance().m_pServer->RemoveSocket(shared_from_this());
		Server_Wrapper::m_mUsers.erase(m_pUser->GetCharacterID());
	}
}

void CConnection::handle_Write(const boost::system::error_code& err, size_t byte_transferred) {

}


void CConnection::start() {	
	time_t now = time(0);	
	m_sMsg = "Hello";
	boost::asio::async_write(m_Socket, boost::asio::buffer(m_sMsg), 
		boost::bind(&CConnection::handle_Accept, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred
		));
}

void CConnection::ProcessPacket(InPacket &iPacket) {
	LONG nType = iPacket.Decode2();

	if (nType >= CGP_User_Start && nType < CGP_User_End) {
		
	}

	switch (nType) {
	case CGP_Login: OnLogin(iPacket); break;
	case CGP_CreateRoom: OnCreateRoom(iPacket); break;
	case CGP_EnterRoom: OnEnterRoom(iPacket); break;
	case CGP_LeaveRoom: OnLeaveRoom(iPacket); break;
	case CGP_GameStart: OnGameStart(iPacket); break;
	case CGP_GameReady: OnGameReady(iPacket); break;
	}
}

void CConnection::SendPacket(OutPacket &oPacket) {
	oPacket.MakeBuf(v);
	boost::asio::async_write(m_Socket, boost::asio::buffer(v),
		boost::bind(&CConnection::handle_Write, shared_from_this(), boost::asio::placeholders::error, boost::asio::placeholders::bytes_transferred
		));
}


void CConnection::OnLogin(InPacket &iPacket) {
	boost::shared_ptr<CUser> pUser(new CUser());
	pUser->SetConnection(m_uSocketSN);
	m_pUser = pUser;
	Server_Wrapper::m_mUsers.insert(std::pair<ULONG, CUser::pointer >(pUser->GetCharacterID(), pUser));
	//SendLoginResult();
}

void CConnection::OnCreateRoom(InPacket &iPacket) {

}

void CConnection::OnEnterRoom(InPacket &iPacket) {

}

void CConnection::OnLeaveRoom(InPacket &iPacket) {

}

void CConnection::OnGameStart(InPacket &iPacket) {

}

void CConnection::OnGameReady(InPacket &iPacket) {

}
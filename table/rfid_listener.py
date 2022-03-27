
from smartcard.CardMonitoring import CardMonitor, CardObserver
from smartcard.util import toHexString
from smartcard.scard import *


# TODO this class is on borrowed time - rewrite in progress!
class RfidListener(object):

    RESPONSE_CODE_NOT_SUPPORTED = [0x6A, 0x81]
    RESPONSE_CODE_OPERATION_FAILED = [0x63, 0x00]

    COMMAND_GET_UID = [0xFF, 0xCA, 0x00, 0x00, 0x00]

    def __init__(self, uid_listener):
        self.listener = uid_listener

    def update(self, observable, actions):
        # This is obviously a bad way to handle exceptions, but I'm rewriting this whole class
        # This is just a temporary band-aid to keep the software from crashing in the meantime
        try:
            (added_cards, removed_cards) = actions
            for card in added_cards:
                # Connect to the reader
                hresult, hcontext = SCardEstablishContext(SCARD_SCOPE_USER)
                hresult, readers = SCardListReaders(hcontext, [])
                reader = readers[0]
                hresult, hcard, dwActiveProtocol = SCardConnect(hcontext,
                                                                reader,
                                                                SCARD_SHARE_SHARED,
                                                                SCARD_PROTOCOL_T0 |
                                                                SCARD_PROTOCOL_T1)

                if hresult != SCARD_S_SUCCESS:
                    print('Error connecting to card reader')
                    pass

                # Connect to the card
                hresult, response = SCardTransmit(hcard,
                                                  dwActiveProtocol,
                                                  RfidListener.COMMAND_GET_UID)

                if hresult != SCARD_S_SUCCESS:
                    print('Error connecting to card reader')
                    pass

                if response == RfidListener.RESPONSE_CODE_NOT_SUPPORTED:
                    print('Error returned from reader: Operation not supported')
                    pass
                elif response == RfidListener.RESPONSE_CODE_OPERATION_FAILED:
                    print('Error returned from reader: Operation failed')
                    pass
                else:
                    uid = str(toHexString(response))
                    # If there is a listener, execute the callback
                    if self.listener:
                        self.listener(uid)
        except:
            print('Exception caught if rfid_listener.py - ignored')
            return

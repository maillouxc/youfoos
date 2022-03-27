#!/bin/bash

# This script sets up the Raspberry Pi 3B+ as needed for our project.
# Please, try to keep this script up to date whenever you make config changes.

starttime=$(date +%s)

# Colors that can be used in the output
OUTPUT_RED='\033[0;31m'
OUTPUT_GREEN='\033[0;32m'
OUTPUT_NO_COLOR='\033[039m'

# Prints the provided string red to indicate an error
error() {
  printf "${OUTPUT_RED}"
  printf "$@\n"
  printf "${OUTPUT_NO_COLOR}"
}

# Just like error(), but exits with exit code 1
exiterror() {
  error "$@"
  endtime=$(date +%s)
  runtime=$((endtime - starttime))
  runtime_minutes=$((runtime/60))
  error "FAILED after ${runtime} seconds (${runtime_minutes} minutes)."
  exit 1
}

# Called when the script finished successfully - prints a message in green
success() {
  printf "${OUTPUT_GREEN}"
  printf "Setup completed successfully; cya!\n"
  endtime=$(date +%s)
  runtime=$((endtime - starttime))
  runtime_minutes=$((runtime/60))
  printf "SUCCESS after ${runtime} seconds (${runtime_minutes} minutes)."
  printf "${OUTPUT_NO_COLOR}"
  exit
}

########## MAIN SCRIPT STARTS HERE ##########

printf "\nNow setting up RPi 3B+ for use with YouFoos\n\n"
sleep 1s

# First make sure the user is root so we can do what we need to
echo "Ensuring you have root privileges..."
if [[ $EUID != 0 ]]
then
  exiterror "This script must be run as root. Please re-run with sudo."
fi

# NOTE: Everything below gcc here is only for the NFC reader
printf "${OUTPUT_GREEN}"
echo "Installing needed apt packages..."
printf "${OUTPUT_NO_COLOR}"
apt-get update || exiterror
apt-get -qy install \
  python3 \
  python3-pip \
  python3-dev \
  python-smbus \
  i2c-tools \
  gcc \
  make \
  swig \
  autoconf \
  debhelper \
  flex \
  libusb-dev \
  libpcsclite-dev \
  libpcsclite1 \
  libccid \
  pcscd \
  pcsc-tools \
  libpcsc-perl \
  libusb-1.0-0-dev \
  libtool \
  libssl-dev \
  cmake \
  checkinstall \
  wget \
  pkg-config || exiterror "Something went wrong installing apt packages"

echo "Installing libnfc..."
cd ~/libnfc
wget https://github.com/nfc-tools/libnfc/releases/download/libnfc-1.7.0-rc7/libnfc-1.7.0-rc7.tar.gz || exiterror
tar -xvzf libnfc-1.7.0-rc7.tar.gz || exiterror
cd libnfc-1.7.0-rc7
make &&
make install

echo "Installing needed Python packages..."
pip3 install \ 
  rpi.gpio \
  Adafruit-LED-Backpack \
  gpiozero \
  jsonpickle \
  pika \
  pyscard || exiterror "Something went wrong installing needed Python packages"

echo -e "Finished installing all needed packages.\n"

# Enable I2C on the Pi
echo "Enabling I2C communication on the Pi..."
if grep -q 'i2c-bcm2708' /etc/modules; then
  echo 'Seems i2c-bcm2708 module already exists, skipping this step.'
else
  echo 'i2c-bcm2708' >> /etc/modules
fi
if grep -q 'i2c-dev' /etc/modules; then
  echo 'Seems i2c-dev module already exists, skipping this step.'
else
  echo 'i2c-dev' >> /etc/modules
fi
if grep -q 'dtparam=i2c1=on' /boot/config.txt; then
  echo 'Seems i2c1 parameter already set, skipping this step.'
else
  echo 'dtparam=i2c1=on' >> /boot/config.txt
fi
if grep -q 'dtparam=i2c_arm=on' /boot/config.txt; then
  echo 'Seems i2c_arm parameter already set, skipping this step.'
else
  echo 'dtparam=i2c_arm=on' >> /boot/config.txt
fi
if [[ -f /etc/modprobe.d/raspi-blacklist.conf ]]; then
  sed -i 's/^blacklist spi-bcm2708/#blacklist spi-bcm2708/' /etc/modprobe.d/raspi-blacklist.conf
  sed -i 's/^blacklist i2c-bcm2708/#blacklist i2c-bcm2708/' /etc/modprobe.d/raspi-blacklist.conf
else
  echo 'File raspi-blacklist.conf does not exist, skip this step.'
fi

success

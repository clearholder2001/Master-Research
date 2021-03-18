PROGRAM_PATH="/data/domain/pcretrieval/web/www/root/program"

# PATH=/usr/local/cuda-7.0/bin:$PATH
LD_LIBRARY_PATH=/data/domain/pcretrieval/web/www/root/program:LD_LIBRARY_PATH

# export PATH
export LD_LIBRARY_PATH

if [ "${2}" == "-d" ]; then
	MONO_LOG_LEVEL=debug xvfb-run --server-args="-screen 0 800x600x24" mono $PROGRAM_PATH/ConsoleKernel.exe "${1}"
elif [ "${2}" != "" ]; then
	xvfb-run --server-args="-screen 0 800x600x24" mono $PROGRAM_PATH/ConsoleKernel.exe "${1}" "${2}"
else
	xvfb-run --server-args="-screen 0 800x600x24" mono $PROGRAM_PATH/ConsoleKernel.exe "${1}"
fi

# :/usr/local/lib:/usr/local/cuda-7.0/lib64

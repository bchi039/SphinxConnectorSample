
#searchd --config /opt/sphinx/conf/sphinx.conf --status

SERVER_OUT=/opt/sphinx/log/searchd.log
TIMEOUT=300

function server_ready() {
    grep -q -F 'accepting connections.' ${SERVER_OUT}
}

function check_sphinx_ready() {
	echo 'Checking sphinxd status'
	for (( i=0; i<${TIMEOUT}; i++ )); do
		sleep 10
		if server_ready; then 
			exit 0
		fi
	done
	exit 1
}

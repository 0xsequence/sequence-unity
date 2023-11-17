package main

import (
	"fmt"
	"net/http"
)

func main() {
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		jsCode := `
			<script>
				function parseUrlParams() {
					var hash = window.location.hash.substr(1);
					var hashParams = hash.split('&');
					var idToken, state;

					for (var i = 0; i < hashParams.length; i++) {
						var p = hashParams[i].split('=');
						switch (p[0]) {
							case 'id_token':
								idToken = p[1];
								break;
							case 'state':
								state = p[1];
								break;
						}
					}

					if (idToken && state) {
						sendCustomUrlScheme(idToken, state);
					} else {
						console.error('id_token or state not found in URL hash fragment');
					}
				}

				function sendCustomUrlScheme(idToken, state) {
					var customURLScheme = 'powered-by-sequence:/oauth2callback#?id_token=' + idToken + '&state=' + state;
					window.location.href = customURLScheme;
					document.body.innerHTML = '<h1 id="returnMessage"><a href="' + customURLScheme + '">Click to return to app</a></h1>';
					document.getElementById("returnMessage").addEventListener("click", function() {
						document.body.innerHTML = '<h1>Please close this page and return to app</h1>';
					});
				}

				window.onload = parseUrlParams;
			</script>
		`

		w.Header().Set("Content-Type", "text/html")
		fmt.Fprint(w, jsCode)
	})

	fmt.Println("Server started on port 8080...")
	http.ListenAndServe(":8080", nil)
}

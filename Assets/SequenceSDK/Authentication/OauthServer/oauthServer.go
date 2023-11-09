package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"net/url"
	"os"
	"strings"
)

func main() {
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		// Extract the relevant query parameters
		authCode := r.URL.Query().Get("code")
		state := r.URL.Query().Get("state")
		redirectURI := "https://d625-70-27-5-107.ngrok-free.app/"
		clientID := "851462432325-q9r1fl22ubes0nhopllpulocfn2fi57h.apps.googleusercontent.com"
		clientSecret := os.Getenv("GOOGLE_CLIENT_SECRET")

		// Perform the token exchange with the authorization server (Google)
		tokenEndpoint := "https://oauth2.googleapis.com/token"
		data := url.Values{
			"code":          {authCode},
			"client_id":     {clientID},
			"redirect_uri":  {redirectURI},
			"grant_type":    {"authorization_code"},
			"client_secret": {clientSecret},
		}

		curlCommand := fmt.Sprintf("curl -X POST '%s' -d '%s'", tokenEndpoint, data.Encode())
		log.Printf("cURL equivalent request: %s\n", curlCommand)

		response, err := http.Post(tokenEndpoint, "application/x-www-form-urlencoded", strings.NewReader(data.Encode()))
		if err != nil {
			http.Error(w, "Token exchange failed with error: "+err.Error(), http.StatusBadRequest)
			return
		}
		defer response.Body.Close()

		// Extract the access token and refresh token from the response
		if response.StatusCode == http.StatusOK {
			var responseBody map[string]interface{}
			err := json.NewDecoder(response.Body).Decode(&responseBody)
			if err != nil {
				http.Error(w, "Failed to decode JSON response", http.StatusInternalServerError)
				return
			}

			idToken, idOK := responseBody["id_token"].(string)
			if !idOK {
				http.Error(w, "Access token not found in response", http.StatusBadRequest)
				return
			}
			accessToken, accessOK := responseBody["access_token"].(string)
			if !accessOK {
				http.Error(w, "Access token not found in response", http.StatusBadRequest)
				return
			}
			refreshToken, _ := responseBody["refresh_token"].(string)

			customURLScheme := fmt.Sprintf("powered-by-sequence:/oauth2callback#?client_id=%s&id_token=%s&access_token=%s&refresh_token=%s&state=%s", clientID, idToken, accessToken, refreshToken, state)
			log.Printf("Custom URL scheme: %s\n", customURLScheme)

			// Send a response back to the client
			responseMessage := fmt.Sprintf(`
				<html>
				<head>
				<script type="text/javascript">
					// Attempt to open the custom URL scheme
					window.location.href = '%s';
				</script>
				</head>
				<body>
				Authentication complete... You can close this page.
				</body>
				</html>`, customURLScheme)

			w.WriteHeader(http.StatusOK)
			w.Header().Set("Content-Type", "text/html")
			_, _ = w.Write([]byte(responseMessage))
		} else {
			http.Error(w, "Token exchange failed", http.StatusBadRequest)
		}
	})

	serverPort := "8080"
	fmt.Printf("Server started on port %s...\n", serverPort)
	err := http.ListenAndServe(":"+serverPort, nil)
	if err != nil {
		fmt.Println("Error starting the server:", err)
		os.Exit(1)
	}
}

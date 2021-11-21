const route = {
    auth: {
        getLoginUrl: "/auth/login"
    },
    playlist: {
        browse: "/playlists/temp",
        recommend: "/playlists/recommend"
    },
    user: {
        getPlaylists: "/users/playlists"
    }
};

export const authRoutes = route.auth;
export const playlistRoutes = route.playlist;
export const userRoutes = route.user;

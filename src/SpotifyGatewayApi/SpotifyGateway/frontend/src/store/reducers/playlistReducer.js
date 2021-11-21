import {
    AddPlaylist,
    ClearPlaylists,
    DropSelectedPlaylist,
    SaveUserPlaylists,
    SaveBrowsePlaylists,
    SelectPlaylist
} from "../../infrastructure/constants/action-constants";

const initState = {
    userPlaylists: null,
    browsePlaylists: null,
    selectedPlaylistId: null
};

const playlistReducer = (state = initState, action) => {
    switch (action.type) {
        case SaveUserPlaylists: {
            return {
                ...state,
                userPlaylists: action.playlists
            };
        }
        case SaveBrowsePlaylists: {
            return {
                ...state,
                browsePlaylists: action.playlists
            };
        }
        case SelectPlaylist: {
            return {
                ...state,
                selectedPlaylistId: state.selectedPlaylistId === action.playlistId ? null : action.playlistId
            };
        }
        case AddPlaylist: {
            if (state.userPlaylists) {
                const playlists = state.userPlaylists.filter(playlist => playlist.id !== action.playlist.id);

                return {
                    ...state,
                    userPlaylists: [...playlists, action.playlist]
                };
            }

            return state;
        }
        case ClearPlaylists: {
            return {
                ...state,
                userPlaylists: null,
                browsePlaylists: null,
                selectedPlaylistId: null
            };
        }
        case DropSelectedPlaylist: {
            return {
                ...state,
                selectedPlaylistId: null
            };
        }
        default:
            return state;
    }
};

export default playlistReducer;

import {
    AddPlaylist,
    ClearPlaylists,
    DropSelectedPlaylist,
    SaveUserPlaylists,
    SaveBrowsePlaylists,
    SelectPlaylist
} from "../../infrastructure/constants/action-constants";

export const saveUserPlaylists = playlists => {
    return {type: SaveUserPlaylists, playlists: playlists};
};

export const saveBrowsePlaylists = playlists => {
    return {type: SaveBrowsePlaylists, playlists: playlists};
};

export const selectPlaylist = playlistId => {
    return {type: SelectPlaylist, playlistId: playlistId};
};

export const addPlaylist = playlist => {
    return {type: AddPlaylist, playlist: playlist};
};

export const clearPlaylists = () => {
    return {type: ClearPlaylists};
};

export const dropSelectedPlaylist = () => {
    return {type: DropSelectedPlaylist};
};

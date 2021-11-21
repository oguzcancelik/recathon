import {get, post} from "./http-wrapper";
import {authRoutes, playlistRoutes, userRoutes} from "../constants/route-constants";
import {requestConfig} from "../constants/request-constants";

export const getLoginUrl = () => {
    return get(authRoutes.getLoginUrl)
};

export const getUserPlaylists = () => {
    return get(userRoutes.getPlaylists);
};

export const getBrowsePlaylists = () => {
    return get(playlistRoutes.browse);
};

export const recommend = data => {
    return post(playlistRoutes.recommend, data, requestConfig);
};

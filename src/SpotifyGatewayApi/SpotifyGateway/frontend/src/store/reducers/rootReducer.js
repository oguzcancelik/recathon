import authReducer from "./authReducer";
import {combineReducers} from "redux";
import playlistReducer from "./playlistReducer";

const rootReducer = combineReducers({
    auth: authReducer,
    playlist: playlistReducer
});

export default rootReducer;

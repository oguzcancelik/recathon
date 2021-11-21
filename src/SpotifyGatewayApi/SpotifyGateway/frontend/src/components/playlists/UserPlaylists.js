import React, {Component} from 'react';
import Loading from "../helpers/Loading";
import {connect} from "react-redux";
import {saveUserPlaylists} from "../../store/actions/playlistActions";
import PlaylistCardContainer from "./PlaylistCardContainer"
import {logout} from "../../store/actions/authActions";
import * as api from "../../infrastructure/api/api-gateway";
import {recommendPrefix} from "../../infrastructure/constants/application-constants";

class UserPlaylists extends Component {

    state = {
        loading: true
    };

    componentDidMount() {
        if (this.props.userPlaylists) {

            this.setState({
                loading: false
            });

            return;
        }

        api.getUserPlaylists()
            .then(res => {
                if (res.status === 200 && !res.data.hasError) {
                    this.props.saveUserPlaylists(res.data.result);
                }

                this.setState({
                    loading: false
                });
            })
            .catch(error => {

                if (error.response.status === 401) {
                    this.props.logout();
                }

                this.setState({
                    loading: false
                });
            });
    }

    render() {
        const userPlaylists = this.props.userPlaylists
            ? this.props.userPlaylists.filter(playlist => !playlist.name.startsWith(recommendPrefix))
            : null;

        return this.state.loading
            ? <Loading text="Getting your playlists"/>
            : this.props.userPlaylists
                ? <PlaylistCardContainer playlists={userPlaylists}/>
                : <Loading text="User has no playlists. We're waiting for you to create a playlist!"/>
    }
}

const mapStateToProps = state => {
    return {
        userPlaylists: state.playlist.userPlaylists
    };
};

const mapDispatchToProps = dispatch => {
    return {
        saveUserPlaylists: playlists => dispatch(saveUserPlaylists(playlists)),
        logout: () => dispatch(logout())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(UserPlaylists);
